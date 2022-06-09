﻿using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_SHW : Dialog_ResourceEditor<SHWSystem>
    {
        //private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_SHW(HoneybeeSchema.SHWSystem shw = default, bool lockedMode = false)
        {
            var sys = shw ?? new SHWSystem($"SHWSystem_{Guid.NewGuid().ToString().Substring(0, 8)}");
            var vm = new SHWViewModel(sys, this);

            //Padding = new Padding(4);
            Title = $"Service Hot Water - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.Padding = new Padding(10);


            //string identifier,
            //string displayName = null,
            //object userData = null,
            //SHWEquipmentType equipmentType = SHWEquipmentType.Gas_WaterHeater,
            //AnyOf<double, Autocalculate> heaterEfficiency = null,
            //AnyOf< double, string> ambientCondition = null,
            //double ambientLossCoefficient = 6.0

            var nameText = new TextBox();

            var eqpType = new EnumDropDown<SHWEquipmentType>();
            var heaterEffAuto = new RadioButton() { Text = "Autocalculate" };
            var heaterEffNumber = new RadioButton();
            var heaterEff = new NumericStepper();

            var ambientCoffConditionRoom = new RadioButton() { Text = "Room ID" };
            var ambientCoffConditionNumber = new RadioButton();
            var ambientCoffCondition = new DoubleText() { Width = 370};
            var ambientCoffConditionRoomID = new TextBox();

            var ambientLossCoefficient = new NumericStepper() {  MaximumDecimalPlaces = 2 };

            nameText.Bind(_ => _.Text, vm, _ => _.Name);
            eqpType.Bind(_=>_.SelectedValue, vm, _ => _.EquipType);


            heaterEffAuto.Bind(c => c.Checked, vm, _ => _.HeaterEffAuto);
            heaterEffNumber.Bind(c => c.Checked, vm, _ => _.HeaterEffEnabled);
            heaterEff.Bind(c => c.Value, vm, _ => _.HeaterEff);
            heaterEff.Bind(c => c.Enabled, vm, _ => _.HeaterEffEnabled);

            ambientCoffConditionNumber.Bind(c => c.Checked, vm, _ => _.AmbientCoffConditionNumberEnabled);
            ambientCoffConditionRoom.Bind(c => c.Checked, vm, _ => _.AmbientCoffConditionRoomIDEnabled);
            //ambientCoffCondition.Bind(c => c.Value, vm, _ => _.AmbientCoffConditionNumber);
            ambientCoffCondition.Bind(c => c.Enabled, vm, _ => _.AmbientCoffConditionNumberEnabled);
            ambientCoffConditionRoomID.Bind(c => c.Text, vm, _ => _.AmbientCoffConditionRoomID);
            ambientCoffConditionRoomID.Bind(c => c.Enabled, vm, _ => _.AmbientCoffConditionRoomIDEnabled);

            ambientCoffCondition.ReservedText = ReservedText.Varies;
            ambientCoffCondition.SetDefault(22);
            ambientCoffCondition.TextBinding.Bind(vm, _ => _.AmbientCoffConditionNumber.NumberText);
            var ambientCoffConditionUnit = new Label();
            ambientCoffConditionUnit.TextBinding.Bind(vm, _ => _.AmbientCoffConditionNumber.DisplayUnitAbbreviation);
     

            ambientLossCoefficient.Bind(_ => _.Value, vm, _ => _.AmbientLostCoff);

            layout.AddRow("Name:");
            layout.AddRow(nameText);
            layout.AddRow("Equipment Type:");
            layout.AddRow(eqpType);

            layout.AddRow("Heater Efficiency:");
            layout.AddRow(heaterEffAuto);
            layout.AddSeparateRow(heaterEffNumber, heaterEff);

            layout.AddSeparateRow("Ambient Condition:");
            layout.AddSeparateRow(ambientCoffConditionNumber, ambientCoffCondition, ambientCoffConditionUnit);
            layout.AddSeparateRow(ambientCoffConditionRoom, ambientCoffConditionRoomID);

            layout.AddRow("Ambient Loss Coefficient [W/K]");
            layout.AddRow(ambientLossCoefficient);

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OKButton = new Button { Text = "OK", Enabled = !lockedMode };
            OKButton.Click += (sender, e) => {
                try
                {
                    OkCommand.Execute(vm.GreateSys(shw));
                }
                catch (Exception er)
                {
                    MessageBox.Show(this, er.Message);
                    //throw;
                }
            }; 

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            var hbData = new Button { Text = "Schema Data" };
            hbData.Click += (sender, e) => Dialog_Message.Show(this, vm.GreateSys(shw).ToJson(true), "Schema Data");

            layout.AddSeparateRow(locked, null, OKButton, this.AbortButton, null, hbData);
            layout.AddRow(null);
            Content = layout;

        }


    }
}