﻿using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class Dialog_Legend : Eto.Forms.Dialog<LegendParameter>
    {
        private LegendViewModel _vm;

        public Dialog_Legend(LegendParameter parameter, Action<LegendParameter> previewAction = default)
        {
            this.Title = $"Legend Parameters - {DialogHelper.PluginName}";
            this.Width = 400;
            this.Icon = DialogHelper.HoneybeeIcon;

            _vm = new LegendViewModel(parameter, this);
            
            var title = new TextBox();
            var x = new Eto.Forms.NumericStepper() { MinValue = 0 };
            var y = new Eto.Forms.NumericStepper() { MinValue = 0 };
            var w = new Eto.Forms.NumericStepper() { MinValue = 1 };
            var h = new Eto.Forms.NumericStepper() { MinValue = 1 };
            var fontH = new Eto.Forms.NumericStepper() { MinValue = 1 };
            var fontColor = new Button();
            var decimalPlaces = new Eto.Forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0 };

            title.TextBinding.Bind(_vm, _=>_.Title);
            x.ValueBinding.Bind(_vm, _=>_.X);
            y.ValueBinding.Bind(_vm, _ => _.Y);
            w.ValueBinding.Bind(_vm, _ => _.W);
            h.ValueBinding.Bind(_vm, _ => _.H);
            fontH.ValueBinding.Bind(_vm, _ => _.FontHeight);
            fontColor.Bind(_=>_.BackgroundColor, _vm, _ => _.FontColor);
            fontColor.Command = _vm.FontColorCommand;
            decimalPlaces.ValueBinding.Bind(_vm, _ => _.DecimalPlaces);

            var minNum = new Eto.Forms.NumericStepper() { MaximumDecimalPlaces = 5 };
            var maxNum = new Eto.Forms.NumericStepper() { MaximumDecimalPlaces = 5 };
            var numSeg = new Eto.Forms.NumericStepper() { DecimalPlaces = 0, MinValue = 1 };
            var continuous = new CheckBox();

            minNum.ValueBinding.Bind(_vm, _ => _.Min);
            maxNum.ValueBinding.Bind(_vm, _ => _.Max);
            numSeg.ValueBinding.Bind(_vm, _=>_.NumSeg);
            continuous.CheckedBinding.Bind(_vm, _=>_.Continuous);
            minNum.Bind(_ => _.Enabled, _vm, _ => _.IsNumberValues);
            maxNum.Bind(_ => _.Enabled, _vm, _ => _.IsNumberValues); 
            numSeg.Bind(_ => _.Enabled, _vm, _ => _.IsNumberValues);
            continuous.Bind(_ => _.Enabled, _vm, _ => _.IsNumberValues);


            var colorPanel = GenColorControl();
            var tb = new TabControl();
            tb.Pages.Add(new TabPage(colorPanel) { Text = "Colors" });

            var general = new DynamicLayout();
            general.Height = 280;
            general.DefaultSpacing = new Eto.Drawing.Size(5, 5);
            general.DefaultPadding = new Eto.Drawing.Padding(5);
            general.AddRow("Font height:", fontH);
            general.AddRow("Font color:", fontColor);
            general.AddRow("Location X:", x);
            general.AddRow("Location Y:", y);
            general.AddRow("Width:", w);
            general.AddRow("Height:", h);
            general.AddRow("Decimal places", decimalPlaces);
            general.AddRow(null, null);
            tb.Pages.Add(new TabPage(general) { Text = "Settings" });

            var OkBtn = new Eto.Forms.Button() { Text = "OK" };
            OkBtn.Click += (s, e) => {
                if (_vm.Validate())
                {
                    var lg = _vm.GetLegend();
                    this.Close(lg);
                }
               
            };
            this.AbortButton = new Eto.Forms.Button() { Text = "Cancel" };
            this.AbortButton.Click += (s, e) => { this.Close(); };

            var preview = new Button() { Text = "Preview", Visible = previewAction != default };
            preview.Click += (s, e) =>
            {
                if (_vm.Validate())
                {
                    var lg = _vm.GetLegend();
                    previewAction?.Invoke(lg);
                }
               
            };

            var layout = new Eto.Forms.DynamicLayout();
            layout.DefaultSpacing = new Eto.Drawing.Size(5, 2);
            layout.DefaultPadding = new Eto.Drawing.Padding(4);
            layout.AddSeparateRow("Legend title:", title);
            layout.AddSeparateRow("Maximum:", maxNum);
            layout.AddSeparateRow("Minimum:", minNum);
            layout.AddSeparateRow("Number of segment:", numSeg);
            layout.AddSeparateRow("Continuous colors:", continuous);

            layout.AddSeparateRow(tb);
      
            //layout.AddSeparateRow("Font height:", fontH, null, "Font color:", fontColor);
            //layout.AddSeparateRow("X:", x, "Y:", y, "W", w, "H", h); 
            layout.AddSeparateRow(null, OkBtn, this.AbortButton, null, preview);
            layout.AddRow(null);
            this.Content = layout;

        }

        private DynamicLayout GenColorControl()
        {
            var layout = new DynamicLayout();
            var layoutCtrl = new DynamicLayout();
            layoutCtrl.DefaultSpacing = new Eto.Drawing.Size(5, 5);
            layoutCtrl.DefaultPadding = new Eto.Drawing.Padding(2, 0, 0, 0);

            var gridView = GenColorPanel();

            var preset = new Button() { Text = "Preset", Width = 45, ToolTip = "Default color presets" };
            var moveUp = new Button() { Text = "↑", Width = 45, ToolTip = "Move a color up" };
            var moveDn = new Button() { Text = "↓", Width = 45, ToolTip = "Move a color down" };
            var flip = new Button() { Text = "Flip", Width = 45, ToolTip = "Flip the order" };
            var add = new Button() { Text = "+", Width = 45, ToolTip = "Add a new color" };
            var remove = new Button() { Text = "x", Width = 45, ToolTip = "Delete a color" };
            var save = new Button() { Text = "Save", Width = 45, ToolTip = "Save as a preset" };

            preset.Command = _vm.PresetCommand;
            moveUp.Command = _vm.MoveUpCommand;
            moveDn.Command = _vm.MoveDownCommand;
            flip.Command = _vm.FlipCommand;
            add.Command = _vm.AddCommand;
            remove.Command = _vm.RemoveCommand;
            save.Command = _vm.SaveCommand;

            moveUp.Bind(_ => _.Enabled, _vm, _ => _.HasColorSelected);
            moveDn.Bind(_ => _.Enabled, _vm, _ => _.HasColorSelected);
            remove.Bind(_ => _.Enabled, _vm, _ => _.HasColorSelected);

            layoutCtrl.AddColumn( preset, save, flip, add, remove, moveUp, moveDn, null);
            layout.AddRow(gridView, layoutCtrl);
            //layout.AddRow(layoutCtrl);
            return layout;
        }

        private GridView GenColorPanel()
        {
           
            var gridView = new GridView();
            gridView.Height = 280;
            gridView.Width = 300;
            gridView.ShowHeader = false;

            gridView.Bind(_ => _.DataStore, _vm, v => v.GridViewDataCollection);
            gridView.SelectedItemsChanged += (s, e) => {
                _vm.SelectedItem = gridView.SelectedItem as ColorDataItem;
            };
            gridView.Bind(_=>_.SelectedRow, _vm, v => v.SelectedRow);

         
            var cTB = new TextBoxCell();
            gridView.Columns.Add(new GridColumn { DataCell = cTB, ID = "Color" });

            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<ColorDataItem, string>(_ => _.Text)
            };
            gridView.Columns.Add(new GridColumn { DataCell = nameTB });

            gridView.CellClick += (s, e) =>
            {
                if (e.Column == 0) 
                    _vm.EditColorsCommand?.Execute(e.Item);
            };

            gridView.CellFormatting += (sender, e) =>
            {
                if (e.Column.ID == "Color")
                {
                    e.BackgroundColor = (e.Item as ColorDataItem).Color;
                    e.ForegroundColor = Eto.Drawing.Color.FromArgb(0, 0, 0);
                }
            };


            return gridView;
        }



    }

}
