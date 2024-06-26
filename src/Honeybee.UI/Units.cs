﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace Honeybee.UI
{
    public static class Units
    {
        public enum UnitType
        {
            Length,
            Area,
            Volume,
            Temperature,
            TemperatureDelta,
            Power,
            Energy,
            PowerDensity,
            AirFlowRate,
            PeopleDensity,
            AirFlowRateArea,
            Speed,
            Illuminance,
            Conductivity,
            Resistance,
            UValue,
            Density,
            SpecificEntropy,
            Dimensionless,
            Pressure,
            EnergyIntensity,
            Mass,
            MassFlow,
            Luminance
        }

        public static Dictionary<UnitType, Enum> _customUnitSettings;

        public static Dictionary<UnitType, Enum> CustomUnitSettings
        {
            get
            {
                if (_customUnitSettings == null)
                    _customUnitSettings = Units.LoadUnits();
                return _customUnitSettings;
            }
            //private set; 
        }
        public static string UnitSettingFile
        {
            get
            {
                if (!System.IO.Directory.Exists(HoneybeeSchema.Helper.Paths.UserAppDataDotnet))
                    System.IO.Directory.CreateDirectory(HoneybeeSchema.Helper.Paths.UserAppDataDotnet);
                var file = System.IO.Path.Combine(HoneybeeSchema.Helper.Paths.UserAppDataDotnet, "UISettings.json");
                return file;
            }
            //private set; 
        }

      

        public static Dictionary<UnitType, Enum> SIUnits = new Dictionary<UnitType, Enum>() {
            { UnitType.Length, LengthUnit.Meter },
            { UnitType.Area, AreaUnit.SquareMeter },
            { UnitType.Volume, VolumeUnit.CubicMeter },
            { UnitType.Temperature, TemperatureUnit.DegreeCelsius },
            { UnitType.TemperatureDelta, TemperatureDeltaUnit.DegreeCelsius },
            { UnitType.Power, PowerUnit.Watt },
            { UnitType.Energy, EnergyUnit.KilowattHour },
            { UnitType.PowerDensity, HeatFluxUnit.WattPerSquareMeter },
            { UnitType.AirFlowRate, VolumeFlowUnit.CubicMeterPerSecond },
            { UnitType.PeopleDensity, ReciprocalAreaUnit.InverseSquareMeter },
            { UnitType.AirFlowRateArea, VolumeFlowPerAreaUnit.CubicMeterPerSecondPerSquareMeter },
            { UnitType.Speed, SpeedUnit.MeterPerSecond },
            { UnitType.Illuminance, IlluminanceUnit.Lux },
            { UnitType.Conductivity, ThermalConductivityUnit.WattPerMeterKelvin },
            { UnitType.Resistance, ThermalResistanceUnit.SquareMeterKelvinPerWatt },
            { UnitType.UValue, HeatTransferCoefficientUnit.WattPerSquareMeterKelvin },
            { UnitType.Density, DensityUnit.KilogramPerCubicMeter },
            { UnitType.SpecificEntropy, SpecificEntropyUnit.JoulePerKilogramKelvin },
            { UnitType.Pressure, PressureUnit.Pascal },
            { UnitType.EnergyIntensity, IrradiationUnit.KilowattHourPerSquareMeter},
            { UnitType.Mass, MassUnit.Kilogram },
            { UnitType.MassFlow, MassFlowUnit.KilogramPerSecond },
            { UnitType.Luminance, LuminanceUnit.CandelaPerSquareMeter},
        };

        public static Dictionary<UnitType, Enum> IPUnits = new Dictionary<UnitType, Enum>() {
            { UnitType.Length, LengthUnit.Foot },
            { UnitType.Area, AreaUnit.SquareFoot },
            { UnitType.Volume, VolumeUnit.CubicFoot },
            { UnitType.Temperature, TemperatureUnit.DegreeFahrenheit },
            { UnitType.TemperatureDelta, TemperatureDeltaUnit.DegreeFahrenheit },
            { UnitType.Power, PowerUnit.BritishThermalUnitPerHour },
            { UnitType.Energy, EnergyUnit.KilobritishThermalUnit },
            { UnitType.PowerDensity, HeatFluxUnit.WattPerSquareFoot },
            { UnitType.AirFlowRate, VolumeFlowUnit.CubicFootPerMinute },
            { UnitType.PeopleDensity, ReciprocalAreaUnit.InverseSquareFoot },
            { UnitType.AirFlowRateArea, VolumeFlowPerAreaUnit.CubicFootPerMinutePerSquareFoot },
            { UnitType.Speed, SpeedUnit.FootPerMinute },
            { UnitType.Illuminance, IlluminanceUnit.Lux },   
            { UnitType.Conductivity, ThermalConductivityUnit.BtuPerHourFootFahrenheit },
            { UnitType.Resistance, ThermalResistanceUnit.HourSquareFeetDegreeFahrenheitPerBtu },
            { UnitType.UValue, HeatTransferCoefficientUnit.BtuPerSquareFootDegreeFahrenheit },
            { UnitType.Density, DensityUnit.PoundPerCubicFoot },
            { UnitType.SpecificEntropy, SpecificEntropyUnit.BtuPerPoundFahrenheit },
            { UnitType.Pressure, PressureUnit.InchOfWaterColumn },
            { UnitType.EnergyIntensity, IrradiationUnit.KilobtuPerSquareFoot },
            { UnitType.Mass, MassUnit.Pound },
            { UnitType.MassFlow, MassFlowUnit.PoundPerSecond },
            { UnitType.Luminance, LuminanceUnit.CandelaPerSquareFoot},
        };

        #region Units

        public enum LengthUnit
        {
            Foot,
            Inch,
            Meter,
            Millimeter,
            Centimeter
        }

        public enum AreaUnit
        {
            SquareFoot,
            SquareInch,
            SquareMeter,
            SquareMillimeter,
            SquareCentimeter,
        }
        public enum VolumeUnit
        {
            CubicFoot,
            CubicInch,
            CubicMeter,
            CubicMillimeter,
            CubicCentimeter,
        }
    
        public enum TemperatureDeltaUnit
        {
            DegreeCelsius,
            DegreeFahrenheit,
        }

        public enum TemperatureUnit
        {
          
            DegreeCelsius,
            DegreeFahrenheit,
            Kelvin

        }
        public enum PowerUnit
        {
            BritishThermalUnitPerHour,
            JoulePerHour,
            GigajoulePerHour,
            Kilowatt,
            Watt,
            KilobritishThermalUnitPerHour,
            MechanicalHorsepower
        }

        public enum EnergyUnit
        {
            KilobritishThermalUnit,
            BritishThermalUnit,
            Joule,
            KilowattHour,
            WattHour,
            Kilojoule,
            Megajoule,
            Gigajoule,
            MegabritishThermalUnit,
            ThermImperial,
            Calorie,
            Kilocalorie

        }

        public enum VolumeFlowUnit
        {
            CubicFootPerMinute,
            CubicMeterPerSecond,
            CubicFootPerSecond,
            LiterPerSecond,
            LiterPerHour,
            UsGallonPerSecond,
            UsGallonPerHour

        }

        public enum HeatFluxUnit
        {
            KilowattPerSquareMeter,
            WattPerSquareFoot,
            WattPerSquareMeter,
            BtuPerHourSquareFoot
        }

        public enum IrradiationUnit
        {
            WattHourPerSquareMeter,
            KilowattHourPerSquareMeter,
            KilobtuPerSquareFoot,
            BtuPerSquareFoot

        }

        public enum ReciprocalAreaUnit
        {
            InverseSquareFoot,
            InverseSquareMeter,
        }

        public enum VolumeFlowPerAreaUnit
        {
            CubicFootPerMinutePerSquareFoot,
            CubicMeterPerSecondPerSquareMeter
        }

        public enum SpeedUnit
        {
            FootPerMinute,
            MilePerHour,
            MeterPerSecond,
            KilometerPerHour,
            FootPerSecond,
            Knot
        }

        public enum IlluminanceUnit
        {
            Lux,
        }

        public enum ThermalConductivityUnit
        {
            BtuPerHourFootFahrenheit,
            WattPerMeterKelvin
        }

        public enum HeatTransferCoefficientUnit
        {
            WattPerSquareMeterKelvin,
            BtuPerSquareFootDegreeFahrenheit
        }

        public enum ThermalResistanceUnit
        {
            HourSquareFeetDegreeFahrenheitPerBtu,
            SquareMeterDegreeCelsiusPerWatt,
            SquareMeterKelvinPerWatt,
        }

        public enum DensityUnit
        {
            KilogramPerCubicMeter,
            PoundPerCubicFoot,
        }
        public enum SpecificEntropyUnit
        {
            BtuPerPoundFahrenheit,
            JoulePerKilogramKelvin,
            KilojoulePerKilogramKelvin,
        }

        public enum PressureUnit
        {
            Pascal,
            InchOfWaterColumn,
            InchOfMercury,
            Bar,
            Atmosphere,
            PoundForcePerSquareInch,
            Torr
        }

        public enum MassFlowUnit
        {
            KilogramPerSecond,
            PoundPerSecond,
            GramPerSecond
        }

        public enum MassUnit
        {
            Kilogram,
            Pound,
            Gram,
            Tonne,
            ShortTon
        }

        public enum LuminanceUnit
        {
            CandelaPerSquareMeter,
            CandelaPerSquareFoot,
        }


        #endregion

        //private static void Test()
        //{
        //    //Honeybee.UI.Units.CustomUnitSettings
        //    UnitsNet.Units.VolumeFlowPerAreaUnit.cu
        //    UnitsNet.HeatFlux.FromBtusPerHourSquareFoot
        //}

        public static void TryAddValue(this Dictionary<UnitType, Enum> CustomUnitSettings, UnitType unitType, Enum value)
        {
            if (CustomUnitSettings == null)
                return;
            if (CustomUnitSettings.ContainsKey(unitType))
            {
                CustomUnitSettings[unitType] = value;
            }
            else
            {
                CustomUnitSettings.Add(unitType, value);
            }
        }

        public static Enum ToUnitsNetEnum(this Enum inputHBEnum)
        {
            if (inputHBEnum == default)
                return inputHBEnum;

            var t = inputHBEnum.GetType().Name.ToString();
            var displayUnitType = typeof(UnitsNet.Angle).Assembly.GetType($"UnitsNet.Units.{t}");
            var ee = Enum.Parse(displayUnitType, inputHBEnum.ToString()) as Enum;
            return ee;
        }

        public static double ConvertValueWithUnits(double value, Enum fromUnit, Enum toUnit)
        {
            if (fromUnit == default || toUnit == default)
                return value;
            if (fromUnit.ToString() == toUnit.ToString())
                return value;


            var quantity = UnitsNet.Quantity.From(value, fromUnit);
            return quantity.As(toUnit);
        }

        public static string GetAbbreviation(this Enum unitsNetEnum)
        {
            var v = Convert.ToInt32(unitsNetEnum);
            var t = unitsNetEnum.GetType();
            return UnitsNetSetup.Default.UnitAbbreviations.GetDefaultAbbreviation(t, v);
        }


      
        internal static void SaveUnits()
        {
            if (CustomUnitSettings == null) return;
            var dic = CustomUnitSettings;
            var js = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            System.IO.File.WriteAllText(UnitSettingFile, js);

        }


        internal static Dictionary<UnitType, Enum> LoadUnits()
        {
            var refDic = SIUnits;
            var units = new Dictionary<UnitType, Enum>();

            if (System.IO.File.Exists(UnitSettingFile))
            {
                var json = System.IO.File.ReadAllText(UnitSettingFile);
                var objs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

                foreach (var item in refDic)
                {
                    var unitKey = item.Key;
                    
                    if(objs.TryGetValue(unitKey.ToString(), out var userV))
                    {
                        var saved = Enum.ToObject(item.Value.GetType(), userV) as Enum;
                        units.Add(unitKey, saved);
                    }
                    else
                    {
                        units.Add(unitKey, item.Value);
                    }
                }

            }
            else
            {
                units =  refDic.ToDictionary(_ => _.Key, _ => _.Value);
            }

          
            return units;


        }

        // helpers
        private static Dictionary<Units.UnitType, (Enum baseUnit, Enum displayUnit)> _mapThermalUnits => new Dictionary<Units.UnitType, (Enum, Enum)>()
        {
            { Units.UnitType.Resistance, (Units.ThermalResistanceUnit.SquareMeterKelvinPerWatt, Units.CustomUnitSettings[Units.UnitType.Resistance])},
            { Units.UnitType.UValue, (Units.HeatTransferCoefficientUnit.WattPerSquareMeterKelvin, Units.CustomUnitSettings[Units.UnitType.UValue])},

        };
        internal static double CheckThermalUnit(Units.UnitType unitType, double num, int round = 5)
        {
            var displayNum = num;
            if (!_mapThermalUnits.TryGetValue(unitType, out var units))
            {
                // do nothing
            }
            else if (!units.baseUnit.Equals(units.displayUnit))
            {
                var baseUnit = units.baseUnit.ToUnitsNetEnum();
                var displayUnit = units.displayUnit.ToUnitsNetEnum();
                displayNum = Units.ConvertValueWithUnits(num, baseUnit, displayUnit);
            }

            return Math.Round(displayNum, round);
        }


        internal static string GetThermalUnitDisplayAbbreviation(Units.UnitType unitType)
        {
            if (!_mapThermalUnits.TryGetValue(unitType, out var units))
            {
                return string.Empty;
            }
            else
            {
                return units.displayUnit.ToUnitsNetEnum().GetAbbreviation();
            }
        }

    }

}
