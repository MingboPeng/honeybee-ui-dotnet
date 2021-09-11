﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
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

        private static HoneybeeSchema.ModelRadianceProperties _systemRadianceLib;
        internal static HoneybeeSchema.ModelRadianceProperties SystemRadianceLib 
        { 
            get 
            {
                if (_systemRadianceLib == null)
                {
                    var rad = HoneybeeSchema.ModelRadianceProperties.Default;
                    //rad.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.StandardRadianceLibrary); 
                    rad.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.UserRadianceLibrary);
                    _systemRadianceLib = rad;
                }
                return _systemRadianceLib;
            } 
        }


        protected void Set(Action setAction, string memberName)
        {
            setAction?.Invoke();
            OnPropertyChanged(memberName);
        }
        void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public void RefreshControl(string memberName)
        {
            OnPropertyChanged(memberName);
        }

        public void RefreshControls(IEnumerable<string> memberNames)
        {
            foreach (var item in memberNames)
            {
                OnPropertyChanged(item);
            }
            
        }
    }



}
