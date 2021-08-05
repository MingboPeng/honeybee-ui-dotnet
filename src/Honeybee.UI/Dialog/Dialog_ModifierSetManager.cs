﻿using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ModifierSetManager : Dialog<List<HoneybeeSchema.Radiance.IBuildingModifierSet>>
    {
        private bool _returnSelectedOnly;
        private ModifierSetManagerViewModel _vm { get; set; }

        private Dialog_ModifierSetManager()
        {
            Padding = new Padding(5);
            Title = $"ModifierSet Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(800, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        public Dialog_ModifierSetManager(ref ModelRadianceProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new ModifierSetManagerViewModel(libSource, this);
            Content = Init();
        }

        private DynamicLayout Init()
        {
            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(5);
            layout.DefaultSpacing = new Size(5, 5);


            var addNew = new Button { Text = "Add" };
            addNew.Command = _vm.AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = _vm.DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = _vm.EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = _vm.RemoveCommand;

            layout.AddSeparateRow("Construction Sets:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddRow(filter);

            var gd = GenGridView();
            layout.AddRow(gd);

            // counts
            var counts = new Label();
            counts.TextBinding.Bind(_vm, _ => _.Counts);
            layout.AddSeparateRow(counts, null);

            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);


            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);
            return layout;
        }

        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as ModifierSetViewData;
            };

            gd.Height = 250;

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierSetViewData, string>(r => r.Name) },
                HeaderText = "Name",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasWallSet) },
                HeaderText = "Wall",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasRoofCeilingSet) },
                HeaderText = "RoofCeiling",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasFloorSet) },
                HeaderText = "Floor",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasApertureSet) },
                HeaderText = "Aperture",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasDoorSet) },
                HeaderText = "Door",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasAirBoundaryModifier) },
                HeaderText = "AirBoundary",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.HasShadeSet) },
                HeaderText = "Shade",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierSetViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierSetViewData, string>(r => r.Source) },
                HeaderText = "Source",
                Sortable = true
            });
            // sorting by header
            gd.ColumnHeaderClick += OnColumnHeaderClick;
            return gd;
        }

        private string _currentSortByColumn;
        private void OnColumnHeaderClick(object sender, GridColumnEventArgs e)
        {
            var cell = e.Column.DataCell;
            var colName = e.Column.HeaderText;
            System.Func<ModifierSetViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (ModifierSetViewData _) => _.Name;
                    break;
                case "Wall":
                    sortFunc = (ModifierSetViewData _) => _.HasWallSet.ToString();
                    break;
                case "RoofCeiling":
                    sortFunc = (ModifierSetViewData _) => _.HasRoofCeilingSet.ToString();
                    break;
                case "Floor":
                    sortFunc = (ModifierSetViewData _) => _.HasFloorSet.ToString();
                    break;
                case "Aperture":
                    sortFunc = (ModifierSetViewData _) => _.HasApertureSet.ToString();
                    break;
                case "Door":
                    sortFunc = (ModifierSetViewData _) => _.HasDoorSet.ToString();
                    break;
                case "AirBoundary":
                    sortFunc = (ModifierSetViewData _) => _.HasAirBoundaryModifier.ToString();
                    break;
                case "Shade":
                    sortFunc = (ModifierSetViewData _) => _.HasShadeSet.ToString();
                    break;
                case "Locked":
                    sortFunc = (ModifierSetViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (ModifierSetViewData _) => _.Source;
                    break;
                default:
                    break;
            }

            if (sortFunc == null) return;

            var descend = colName == _currentSortByColumn;
            _vm.SortList(sortFunc, isNumber, descend);

            _currentSortByColumn = colName == _currentSortByColumn ? string.Empty : colName;

        }


        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var itemsToReturn = _vm.GetUserItems(_returnSelectedOnly);
            Close(itemsToReturn);
        });

    }
}
