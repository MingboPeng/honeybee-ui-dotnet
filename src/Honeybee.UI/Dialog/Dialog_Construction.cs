﻿using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HoneybeeSchema;
using LadybugDisplaySchema;
using System.Xml;

namespace Honeybee.UI
{

    public class Dialog_Construction : Dialog_ResourceEditor<HB.Energy.IConstruction>
    {
        private Label _r_label;
        private Label _r_value;
        private Label _u_label;
        private Label _u_value;
        private Label _uf_label;
        private Label _uf_value;
        const string _r = "R Value";
        const string _u = "U Value";
        const string _uf = "U Factor";

        private string _rDisplayUnit = Units.GetThermalUnitDisplayAbbreviation(Units.UnitType.Resistance);
        private string _uDisplayUnit = Units.GetThermalUnitDisplayAbbreviation(Units.UnitType.UValue);

        //private List<string> _layers = new List<string>();
        private HB.Energy.IConstruction _hbObj;
        private DynamicLayout _layersPanel;

        public List<string> _layers
        {
            get {
                if (_hbObj is HB.OpaqueConstructionAbridged obj)
                {
                    return obj.Materials;
                }
                else if (_hbObj is HB.WindowConstructionAbridged win)
                {
                    return win.Materials;
                }
                return new List<string>();
              
            }
            set 
            {
                if (_hbObj is HB.OpaqueConstructionAbridged obj)
                {
                    obj.Materials = value;
                }
                else if (_hbObj is HB.WindowConstructionAbridged win)
                {
                    win.Materials = value;
                }
                
            }
        }

        private string MakeUserReadable(string materialId)
        {
            IEnumerable<HB.Energy.IMaterial> searchBase = OpaqueMaterials;

            if (_hbObj is HB.OpaqueConstructionAbridged obj)
            {
                // do nothing..
            }
            else if (_hbObj is HB.WindowConstructionAbridged win)
            {
                searchBase = WindowMaterials;
            }

            var found = searchBase.FirstOrDefault(_ => _.Identifier == materialId);
            if (found == null)
                return materialId;
            return found.DisplayName ?? found.Identifier;
        }

        private static HoneybeeSchema.ModelEnergyProperties _systemEnergyLib;
        internal static HoneybeeSchema.ModelEnergyProperties SystemEnergyLib
        {
            get
            {
                if (_systemEnergyLib == null)
                {
                    var eng = HoneybeeSchema.ModelEnergyProperties.Default;
                    eng.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.StandardEnergyLibrary);
                    eng.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.UserEnergyLibrary);
                    _systemEnergyLib = eng;
                }
                return _systemEnergyLib;
            }
        }
        private IEnumerable<HB.Energy.IMaterial> _opaqueMaterials;

        public IEnumerable<HB.Energy.IMaterial> OpaqueMaterials
        {
            get {

                if (_opaqueMaterials == null)
                {
                    var libObjs = SystemEnergyLib.MaterialList.OfType<HB.Energy.IOpaqueMaterial>();
                    var inModelObjs = this.ModelEnergyProperties.MaterialList.OfType<HB.Energy.IOpaqueMaterial>();

                    libObjs = libObjs.Concat(inModelObjs);
                    _opaqueMaterials = libObjs;
                }
               
                return _opaqueMaterials; 
            }
        }

        private IEnumerable<HB.Energy.IMaterial> _windowMaterials;
        public IEnumerable<HB.Energy.IMaterial> WindowMaterials
        {
            get
            {
                if (_windowMaterials == null)
                {
                    var libObjs = SystemEnergyLib.MaterialList.Where(_ => !(_ is HB.Energy.IOpaqueMaterial));
                    var inModelObjs = this.ModelEnergyProperties.MaterialList.Where(_ => !(_ is HB.Energy.IOpaqueMaterial));

                    libObjs = libObjs.Concat(inModelObjs);

                    _windowMaterials = libObjs;
                }
                
                return _windowMaterials;
            }
        }

        private HB.ModelEnergyProperties ModelEnergyProperties { get; set; }
        private bool _isIDEditable;
        public Dialog_Construction(HB.ModelEnergyProperties libSource, HB.Energy.IConstruction construction, bool lockedMode = false, bool editID = false)
        {
            try
            {
                this.ModelEnergyProperties = libSource;

                _hbObj = construction;
                _isIDEditable = editID;
                
                Padding = new Padding(5);
                Resizable = true;
                Title = $"Construction - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 500);
                this.Icon = DialogHelper.HoneybeeIcon;

                var OkButton = new Button { Text = "OK" , Enabled = !lockedMode};
                OkButton.Click += (sender, e) =>
                {
                    try
                    {
                        // add new material to library source
                        foreach (var layer in _layers)
                        {
                            var m = OpaqueMaterials.FirstOrDefault(_ => _.Identifier == layer);
                            m = m ?? WindowMaterials.FirstOrDefault(_ => _.Identifier == layer);
                            var dup = m.Duplicate() as HB.Energy.IMaterial;
                            dup.DisplayName = m.DisplayName ?? m.Identifier;
                            libSource.AddMaterial(dup);
                        }
                        OkCommand.Execute(_hbObj);
                    }
                    catch (Exception ex)
                    {
                        Dialog_Message.Show(this, ex);
                    }
                  
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                //var panel = new Panel_Schedule(sch);
                //panel.AddSeparateRow(null, OkButton, AbortButton, null);
                //Content = panel;

                var leftLayout = new DynamicLayout();
                leftLayout.DefaultSpacing = new Size(5, 5);
                leftLayout.DefaultPadding = new Padding(5);
                leftLayout.Height = 600;

                var hbObjType = _hbObj.GetType();

                // Identifier
                var idPanel = DialogHelper.MakeIDEditor(_hbObj, _isIDEditable);
                var idLabel = new Label() { Text = "ID " };
                var idDescription = HoneybeeSchema.SummaryAttribute.GetSummary(hbObjType, nameof(_hbObj.Identifier));
                idLabel.ToolTip = Utility.NiceDescription(idDescription);
           
                var name = new TextBox();
                _hbObj.DisplayName = _hbObj.DisplayName ?? _hbObj.Identifier;
                name.TextBinding.Bind(() => _hbObj.DisplayName, v => _hbObj.DisplayName = v);
           

                var panelName = new DynamicLayout();
                panelName.DefaultSpacing = new Size(0, 5);
                panelName.AddRow(idLabel, idPanel);
                panelName.AddRow("Name ", name);
                leftLayout.AddRow(panelName);

                var groupContent = new DynamicLayout();
                groupContent.DefaultSpacing = new Size(5, 5);
                Action<int, string> actionWhenItemChanged = (int layerIndex, string newValue) => {
                    _layers.RemoveAt(layerIndex);
                    _layers.Insert(layerIndex, newValue);
                    CalRValue(_hbObj);
                };

                _layersPanel = new DynamicLayout();
                _layersPanel.DefaultSpacing = new Size(5, 5);
                GenMaterialLayersPanel(_layersPanel, actionWhenItemChanged);
                var fromLibDropin = GenAddMoreDropInArea(
                  (newValue) =>
                  {
                      var newIndex = _layers.Count;
                      _layers.Add(newValue);

                      GenMaterialLayersPanel(_layersPanel, actionWhenItemChanged);
                      _layersPanel.Create();
                      CalRValue(_hbObj);
                  }
                  );
                groupContent.AddSeparateRow(null, "Outside", null);
                groupContent.AddRow(_layersPanel);
                groupContent.AddRow(fromLibDropin);
                groupContent.AddSeparateRow(null, "Inside", null);
                //group.Content = groupContent;
                leftLayout.AddRow(groupContent);
                leftLayout.AddRow(null);
                //leftLayout.AddAutoSized(new Button());

                //leftLayout.AddSeparateRow(new Button());
                // Thermal properties
                var thermGp = new GroupBox() { Text = "Construction Thermal Properties" };
                var thermProp = new DynamicLayout() { DefaultPadding = new Padding(4)};
                //thermProp.AddSeparateRow("Construction Thermal Properties", null);
                _r_label = new Label() { Text = _r };
                _u_label = new Label() { Text = _u };
                _uf_label = new Label() { Text = _uf };
                _r_value = new Label() { Text = "Not available" };
                _u_value = new Label() { Text = "Not available" };
                _uf_value = new Label() { Text = "Not available" };
                thermProp.AddSeparateRow(_r_label, ":", _r_value);
                thermProp.AddSeparateRow(_u_label, ":", _u_value);
                thermProp.AddSeparateRow(_uf_label, ":", _uf_value);
                CalRValue(_hbObj);

                thermGp.Content = thermProp;
                leftLayout.AddRow(thermGp);
                leftLayout.AddRow(null);

                var buttonSource = new Button { Text = "Schema Data" };
                buttonSource.Click += (s, e) =>
                {
                    Dialog_Message.ShowFullMessage(this, _hbObj.ToJson(true));
                };
              

                //leftLayout.AddRow(null);

                //Right panel
                var rightGroup = new GroupBox();
                rightGroup.Text = "Library";
                var groupPanel = new DynamicLayout();


                var materialType = new DropDown();
                materialType.Items.Add(new ListItem() { Key = "Opaque", Text = "Opaque Material" });
                materialType.Items.Add(new ListItem() { Key = "Window", Text = "Window Material" });
                //constructionTypes.Items.Add(new ListItem() { Key = "Shade Material" });
                //constructionTypes.Items.Add(new ListItem() { Key = "AirBoundary Material" });
                materialType.SelectedIndex = 0;
                groupPanel.AddRow(materialType);

                //Search tbox
                var searchTBox = new TextBox() { PlaceholderText = "Search" };
                groupPanel.AddRow(searchTBox);

                var allMaterials = OpaqueMaterials;

                // Library
                var lib = new GridView();
                lib.Height = 400;
                groupPanel.AddRow(lib);

                lib.ShowHeader = false;
                var nameCol = new GridColumn() { DataCell = new TextBoxCell(0) };
                lib.Columns.Add(nameCol);

                // Library items
                lib.DataStore = allMaterials;

               
                var idCell = new TextBoxCell
                {
                    Binding = Binding.Delegate<HB.Energy.IIDdEnergyBaseModel, string>(r => r.DisplayName ?? r.Identifier)
                };
                lib.Columns.Add(new GridColumn() { DataCell = idCell });


                

                // material details
                var detailPanel = new DynamicLayout();
                var materialDetail = new TextArea();
                materialDetail.Height = 150;
                materialDetail.Text = "Material Details";
                //groupPanel.AddRow(materialDetail);

                var rightSplit = new Splitter();
                rightSplit.Panel1 = groupPanel;
                rightSplit.Panel2 = materialDetail;
                rightSplit.Panel1MinimumSize = 300;
                rightSplit.Orientation = Orientation.Vertical;

                rightGroup.Content = rightSplit;


                materialType.SelectedIndexChanged += (sender, e) =>
                {
                    var selectedType = materialType.SelectedKey;
                    allMaterials = selectedType == "Window" ? this.WindowMaterials : this.OpaqueMaterials;
                    searchTBox.Text = null;
                    //lib.Items.Clear();

                    lib.DataStore = allMaterials;
                    //var filteredItems = allMaterials.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                    //lib.Items.AddRange(filteredItems);

                };



                //// need this to make drag drop work on Mac
                //lib.MouseDown += (sender, e) => {
                //    _primMousePressed = e.Buttons == MouseButtons.Primary;
                //};
                //lib.MouseUp += (sender, e) => {
                //    _primMousePressed = false;

                //};
                
                lib.MouseMove += (sender, e) =>
                {
                    if (e.Buttons != MouseButtons.Primary)
                        return;
                    if (lib.SelectedItem == null)
                        return;

                    var dragableArea = lib.Bounds;
                    dragableArea.Width -= 20;
                    dragableArea.Height -= 20;
                    var iscontained = e.Location.Y < dragableArea.Height  && e.Location.X < dragableArea.Width;
                    //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                    if (!iscontained)
                        return;


                    var cell = lib.GetCellAt(e.Location);
                    if (cell.RowIndex == -1 || cell.ColumnIndex == -1)
                        return;

                    var selected = (lib.SelectedItem as HB.Energy.IMaterial).Identifier;
                    var data = new DataObject();
                    data.SetString(selected, "HBObj");
                    lib.DoDragDrop(data, DragEffects.Move);
                    e.Handled = true;
                };

                lib.SelectedItemsChanged += (s, e) => 
                {
                    //Clear preview first
                    materialDetail.Text = null;

                    //Check current selected item from library
                    var selItem = lib.SelectedItem as HB.IHoneybeeObject;
                    if (selItem == null)
                        return;

                    //Update Preview
                    materialDetail.Text = selItem.ToString(true);

                };

               
                searchTBox.TextChanged += (sender, e) =>
                {
                    var input = searchTBox.Text;
                    materialDetail.Text = null;

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        lib.DataStore = allMaterials;
                        return;
                    }
                    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                    var filtered = allMaterials.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null ? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
               
                    lib.DataStore = filtered;

                };





                //var split = new Splitter();
                //split.Orientation = Orientation.Horizontal;
                //split.Panel1 = leftLayout;
                //split.Panel2 = rightGroup;

                var locked = new CheckBox() { Text = "Locked", Enabled = false };
                locked.Checked = lockedMode;

                var layout = new DynamicLayout();
                layout.DefaultSpacing = new Size(5, 5);
                layout.DefaultPadding = new Padding(5);
                layout.AddSeparateRow(leftLayout, rightGroup);
                layout.AddSeparateRow(locked, null, OkButton, AbortButton, null, buttonSource);
                layout.AddRow(null);

                //Create layout
                Content = layout;

            }
            catch (Exception e)
            {
                Dialog_Message.Show(this, e);
            }
            
            
        }

        private void GenMaterialLayersPanel(DynamicLayout layout, Action<int, string> actionAfterChanged)
        {
            _layersPanel.Clear();
            var index = 0;
            foreach (var item in _layers)
            {
                //var id = Guid.NewGuid();
                var dropin = GenDropInArea(index, item, actionAfterChanged,
                    (i) => {
                        _layers.RemoveAt(i);
                        ////ctrls.Detach();
                        //ctrls.Clear();
                        GenMaterialLayersPanel(layout, actionAfterChanged);
                        //foreach (var c in newPanel.Controls)
                        //{
                        //    ctrlsLayers.AddRow(c);
                        //}

                        //ctrls.AddRow(new TextBox());
                        _layersPanel.Create();
                        CalRValue(_hbObj);

                    });
                _layersPanel.AddRow(dropin);

                index++;
            }
            //return ctrls;

            //GenItems();
            ////return ctrls;

            //void GenItems()
            //{
            //    _layersPanel.Clear();
            //    var index = 0;
            //    foreach (var item in _layers)
            //    {
            //        var id = Guid.NewGuid();
            //        var dropin = GenDropInArea(index, item, actionAfterChanged,
            //            (i) => {
            //                _layers.RemoveAt(i);
            //                ////ctrls.Detach();
            //                //ctrls.Clear();
            //                GenMaterialLayersPanel(layout, actionAfterChanged);
            //                //foreach (var c in newPanel.Controls)
            //                //{
            //                //    ctrlsLayers.AddRow(c);
            //                //}

            //                //ctrls.AddRow(new TextBox());
            //                ctrls.Create();

            //            });
            //        ctrls.AddRow(dropin);

            //        index++;
            //    }
            //    //return ctrls;
            //}
        
            //ctrls.EndGroup();
        }

        private int _leftControlsWith = 300;
        private Control GenDropInArea(int layerIndex, string text, Action<int, string> actionAfterChanged, Action<int> deleteAction)
        {
            var width = _leftControlsWith;
            var height = 36;

            var layerPanel = new PixelLayout();


            var dropInValue = new TextBox();
            var materialDisplayText = MakeUserReadable(text);
            dropInValue.Text = materialDisplayText ?? "Drag from library";
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.Width = width;
            dropInValue.Height = height;
            dropInValue.Enabled = false;
            dropInValue.BackgroundColor = Colors.Transparent;
   

            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = width;
            dropIn.Height = height;
            dropIn.BackgroundColor = Colors.Transparent;

            var deleteBtn = new Button();
            deleteBtn.Text = "✕";
            deleteBtn.Width = 20;
            deleteBtn.Height = 20;
            deleteBtn.Click += (s, e) => deleteAction(layerIndex);
            //deleteBtn.Visible = false;

            //dropIn.MouseMove += (sender, e) =>
            //{
            //    deleteBtn.Visible = true;
            //    //dropInValue.BackgroundColor = Colors.White;
            //};
            //dropIn.MouseLeave += (sender, e) =>
            //{
            //    deleteBtn.Visible = false;
            //};
            //dropIn.Paint += (s, e) =>
            //{
            //    e.Graphics.DrawText(font, brush, new PointF(0, 0), item);
            //};
            //dropIn.DragEnter += (sender, e) =>
            //{
            //    //if (e.Effects != DragEffects.None)
            //    dropIn.BackgroundColor = Colors.Green;
            //};
            dropIn.DragLeave += (sender, e) =>
            {
                dropInValue.BackgroundColor = Colors.Transparent;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValue.BackgroundColor = Colors.LightGrey;

            };
            dropIn.DragDrop += (sender, e) =>
            {
                //if (e.Effects == DragEffects.None)
                //    return;
                //e.Effects = DragEffects.All;
                //dropIn.BackgroundColor = Colors.Red;
                var newValue = e.Data.GetString("HBObj");
                dropInValue.Text = MakeUserReadable(newValue); ;
                actionAfterChanged(layerIndex, newValue);

            };
            //layerPanel.Add(backgroundLayer, 0, 0);
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            layerPanel.Add(deleteBtn, width - 26, 8);
            return layerPanel;
        }
        private Control GenAddMoreDropInArea(Action<string> actionAfterNewDroppedIn)
        {
            var width = _leftControlsWith;
            var height = 60;

            var layerPanel = new PixelLayout();
            var dropInValue = new Label();
            dropInValue.Text = "Drag a new material from library";
            dropInValue.TextColor = Colors.DimGray;
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.VerticalAlignment = VerticalAlignment.Center;

            dropInValue.Width = width;
            dropInValue.Height = height;
            var backGround =  Eto.Drawing.Color.FromArgb(230, 230, 230);
            dropInValue.BackgroundColor = backGround;

            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = width;
            dropIn.Height = height;
            dropIn.BackgroundColor = Colors.Transparent;

            dropIn.DragLeave += (sender, e) =>
            {

                dropInValue.BackgroundColor = backGround;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValue.BackgroundColor = Colors.LightGrey;
            };
            dropIn.DragDrop += (sender, e) =>
            {
              
                var newValue = e.Data.GetString("HBObj");
                //dropInValue.Text = newValue;
                actionAfterNewDroppedIn(newValue);

            };
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            return layerPanel;
        }

        void CalRValue(HB.Energy.IConstruction c)
        {
            if (c is HB.Energy.IThermalConstruction tc)
            {
                if (_layers.Count == 0)
                {
                    var notAva = "Not available";
                    _r_value.Text = notAva;
                    _u_value.Text = notAva;
                    _uf_value.Text = notAva;
                    _r_label.Text = notAva;
                    _u_label.Text = notAva;
                    _uf_label.Text = notAva;
                    return;
                }
                   

                var dupLib = this.ModelEnergyProperties.DuplicateModelEnergyProperties();
                // add all materials to lib
                foreach (var layer in _layers)
                {
                    var m = OpaqueMaterials.FirstOrDefault(_ => _.Identifier == layer);
                    m = m ?? WindowMaterials.FirstOrDefault(_ => _.Identifier == layer);
                    var dup = m.Duplicate() as HB.Energy.IMaterial;
                    dupLib.AddMaterial(dup);
                }

                var valid = tc.CalThermalValues(dupLib);

                var r = Units.CheckThermalUnit(Units.UnitType.Resistance, tc.RValue);
                var u = Units.CheckThermalUnit(Units.UnitType.UValue, tc.UValue);
                var uf = Units.CheckThermalUnit(Units.UnitType.UValue, tc.UFactor);

                var rText = valid ? (r < 0 ? "Skylight only" : r.ToString()) : "Invalid";
                var uText = valid ? (u < 0 ? "Skylight only" : u.ToString()) : "Invalid";
                var ufText = valid ? uf.ToString() : "Invalid";

                _r_value.Text = rText;
                _u_value.Text = uText;
                _uf_value.Text = ufText;
                
                _r_label.Text =  $"{_r} [{_rDisplayUnit}]";
                _u_label.Text = $"{_u} [{_uDisplayUnit}]";
                _uf_label.Text = $"{_uf} [{_uDisplayUnit}]";
            }

        }
    }
}
