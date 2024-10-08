﻿using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class SetpointViewModel : CheckboxPanelViewModel
    {
        private SetpointAbridged _refHBObj => this.refObjProperty as SetpointAbridged;
        // string coolingSchedule, string heatingSchedule, string displayName = null, string humidifyingSchedule = null, string dehumidifyingSchedule = null

        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            set
            {
                _isDisplayNameVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName));
            }
        }

        // CoolingSchedule
        private ButtonViewModel _coolingSchedule;

        public ButtonViewModel CoolingSchedule
        {
            get => _coolingSchedule;
            set { this.Set(() => _coolingSchedule = value, nameof(CoolingSchedule)); }
        }


        // HeatingSchedule
        private ButtonViewModel _heatingSchedule;

        public ButtonViewModel HeatingSchedule
        {
            get => _heatingSchedule;
            set { this.Set(() => _heatingSchedule = value, nameof(HeatingSchedule)); }
        }

        // HumidifyingSchedule
        private OptionalButtonViewModel _humidifyingSchedule;

        public OptionalButtonViewModel HumidifyingSchedule
        {
            get => _humidifyingSchedule;
            set { this.Set(() => _humidifyingSchedule = value, nameof(HumidifyingSchedule)); }
        }

        // dehumidifyingSchedule
        private OptionalButtonViewModel _dehumidifyingSchedule;

        public OptionalButtonViewModel DehumidifyingSchedule
        {
            get => _dehumidifyingSchedule;
            set { this.Set(() => _dehumidifyingSchedule = value, nameof(DehumidifyingSchedule)); }
        }


        public SetpointAbridged Default { get; private set; }
        public SetpointViewModel(ModelProperties libSource, List<SetpointAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new SetpointAbridged(Guid.NewGuid().ToString().Substring(0, 5), ReservedText.None, ReservedText.None);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateSetpointAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateSetpointAbridged();


            if (loads.Distinct().Count() == 1) 
                this.IsCheckboxChecked = loads.FirstOrDefault() == null;
            else
                this.IsCheckboxVaries();


            //DisplayName
            if (loads.Select(_ => _?.DisplayName).Distinct().Count() > 1)
                this.DisplayName = ReservedText.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;


            //CoolingSchedule
            var clSch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.CoolingSchedule);
            clSch = clSch ?? GetDummyScheduleObj(_refHBObj.CoolingSchedule);
            this.CoolingSchedule = new ButtonViewModel((n) => _refHBObj.CoolingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.CoolingSchedule).Distinct().Count() > 1)
                this.CoolingSchedule.SetBtnName(ReservedText.Varies);
            else
                this.CoolingSchedule.SetPropetyObj(clSch);


            //HeatingSchedule
            var htSch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.HeatingSchedule);
            htSch = htSch ?? GetDummyScheduleObj(_refHBObj.HeatingSchedule);
            this.HeatingSchedule = new ButtonViewModel((n) => _refHBObj.HeatingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.HeatingSchedule).Distinct().Count() > 1)
                this.HeatingSchedule.SetBtnName(ReservedText.Varies);
            else
                this.HeatingSchedule.SetPropetyObj(htSch);


            //HumidifyingSchedule
            var huSch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.HumidifyingSchedule);
            huSch = huSch ?? GetDummyScheduleObj(_refHBObj.HumidifyingSchedule);
            this.HumidifyingSchedule = new OptionalButtonViewModel((n) => _refHBObj.HumidifyingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.HumidifyingSchedule).Distinct().Count() > 1)
                this.HumidifyingSchedule.SetBtnName(ReservedText.Varies);
            else
                this.HumidifyingSchedule.SetPropetyObj(huSch);


            //DehumidifyingSchedule
            var dhSch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.DehumidifyingSchedule);
            dhSch = dhSch ?? GetDummyScheduleObj(_refHBObj.DehumidifyingSchedule);
            this.DehumidifyingSchedule = new OptionalButtonViewModel((n) => _refHBObj.DehumidifyingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.DehumidifyingSchedule).Distinct().Count() > 1)
                this.DehumidifyingSchedule.SetBtnName(ReservedText.Varies);
            else
                this.DehumidifyingSchedule.SetPropetyObj(dhSch);

            

        }

        public SetpointAbridged MatchObj(SetpointAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked.GetValueOrDefault())
                return null;

            if (this.IsVaries)
                return obj?.DuplicateSetpointAbridged();

            obj = obj?.DuplicateSetpointAbridged() ?? new SetpointAbridged(Guid.NewGuid().ToString().Substring(0, 5), ReservedText.NotSet, ReservedText.NotSet);


            if (!this._isDisplayNameVaries)
                obj.DisplayName = this._refHBObj.DisplayName;

            if (!this.CoolingSchedule.IsVaries)
            {
                if (this._refHBObj.CoolingSchedule == null)
                    throw new ArgumentException("Missing a required setpoint cooling schedule!");
                obj.CoolingSchedule = this._refHBObj.CoolingSchedule;
            }
       
            if (!this.HeatingSchedule.IsVaries)
            {
                if (this._refHBObj.HeatingSchedule == null)
                    throw new ArgumentException("Missing a required setpoint heating schedule!");
                obj.HeatingSchedule = this._refHBObj.HeatingSchedule;
            }

            if (!this.HumidifyingSchedule.IsVaries)
                obj.HumidifyingSchedule = this._refHBObj.HumidifyingSchedule;

            if (!this.DehumidifyingSchedule.IsVaries)
                obj.DehumidifyingSchedule = this._refHBObj.DehumidifyingSchedule;


            return obj;
        }

        public RelayCommand CoolingScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.CoolingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });


        public RelayCommand HeatingScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HeatingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand HumidifyingScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HumidifyingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });
        public RelayCommand RemoveHumidifyingScheduleCommand => new RelayCommand(() =>
        {
            this.HumidifyingSchedule.SetPropetyObj(null);
        });

        public RelayCommand DehumidifyingScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.DehumidifyingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });
        public RelayCommand RemoveDehumidifyingScheduleCommand => new RelayCommand(() =>
        {
            this.DehumidifyingSchedule.SetPropetyObj(null);
        });

    }


}
