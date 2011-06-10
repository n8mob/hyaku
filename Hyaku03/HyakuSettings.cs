using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NateGrigg.Mobile.Utility;
using System.Collections.Generic;
using System.ComponentModel;

namespace Hyaku
{
    public class HyakuSettings : IsolatedAppSettings
    {
        // The isolated storage key names of our settings
        const string GameSizeSettingKeyName = "GameSizeSetting";
        const string EnableJunkRowsKeyName = "EnableJunkRowsSetting";
        const string AutoDropPeriodKeyName = "AutoDropPeriodSetting";
        const string SweepTimerPeriodKeyName = "SweepTimerPeriodSetting";
        const string TimerTickIntervalKeyName = "TimerTickIntervalSetting";
        const string NumberListStringKeyName = "NumberListStringSetting";
        const string UseDebugNumbersKeyName = "UseDebugNumbers";
        const string DebugNumbersStringKeyName = "DebugNumbers";
        const string MaxDistanceKeyName = "MaxDistance";

        // Default settings values
        const int GameSizeSettingDefault = 9;
        const bool EnableJunkRowsDefault = false;
        const int AutoDropPeriodDefault = 30;
        const int SweepTimerPeriodDefault = 100;
        const int TimerTickIntervalDefault = 300; // milliseconds
        const string NumberListStringDefault = "5,5,5,10,10,10,15,15,15,20,20,20,25,25,25,30,30,30,35,35,40,40,45,45,50,50,55,55,60,60,65,65,70,75,80,85,90,95";
        const bool UseDebugNumbersDefault = false;
        const string DebugNumbersStringDefault = "";
        const int MaxDistanceDefault = 2;

        public int GameSizeSetting
        {
            get
            {
                return GetValueOrDefault<int>(GameSizeSettingKeyName, GameSizeSettingDefault);
            }
            set
            {
                if (5 <= value && value <= 13) {
                    AddOrUpdateValue(GameSizeSettingKeyName, value);
                    Save();
                }
            }
        }

        public bool EnableJunkRowsSetting
        {
            get
            {
                return GetValueOrDefault<bool>(EnableJunkRowsKeyName, EnableJunkRowsDefault);
            }
            set
            {
                AddOrUpdateValue(EnableJunkRowsKeyName, value);
                Save();
            }
        }

        public int SweepTimerPeriodSetting
        {
            get
            {
                return GetValueOrDefault<int>(SweepTimerPeriodKeyName, SweepTimerPeriodDefault);
            }
            set
            {
                if (1 <= value && value <= 360) {
                    AddOrUpdateValue(SweepTimerPeriodKeyName, value);
                    Save();
                }
            }
        }

        /// <summary>
        /// Timer tick interval in milliseconds
        /// </summary>
        public int TimerTickIntervalSetting
        {
            get
            {
                return GetValueOrDefault<int>(TimerTickIntervalKeyName, TimerTickIntervalDefault);
            }
            set
            {
                AddOrUpdateValue(TimerTickIntervalKeyName, value);
            }
        }

        public string NumberListStringSetting
        {
            get
            {
                return GetValueOrDefault<string>(NumberListStringKeyName, NumberListStringDefault);
            }
            set
            {
                AddOrUpdateValue(NumberListStringKeyName, value);
            }
        }

        public List<int> NumberListSetting
        {
            get
            {
                string[] listString = NumberListStringSetting.Split(',');
                List<int> listInts = (from s in listString
                                  select int.Parse(s)).ToList();
                return listInts;
            }
            set
            {
                string[] listString = (from i in value
                                       select i.ToString()).ToArray();
                string list = string.Join(",", listString);
                AddOrUpdateValue(NumberListStringKeyName, list);
            }
        }

        public bool UseDebugNumbersSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UseDebugNumbersKeyName, UseDebugNumbersDefault);
            }
            set
            {
                AddOrUpdateValue(UseDebugNumbersKeyName, value);
            }
        }

        public string DebugNumbersStringSetting
        {
            get
            {
                return GetValueOrDefault<string>(DebugNumbersStringKeyName, DebugNumbersStringDefault);
            }
            set
            {
                AddOrUpdateValue(DebugNumbersStringKeyName, value);
            }
        }

        public List<int> DebugNumbersSetting
        {
            get
            {
                string[] listString = DebugNumbersStringSetting.Split(',');
                List<int> listInts = (from s in listString
                                      select int.Parse(s)).ToList();
                return listInts;
            }
            set
            {
                string[] listString = (from i in value
                                       select i.ToString()).ToArray();
                string list = string.Join(",", listString);
                AddOrUpdateValue(DebugNumbersStringKeyName, list);
            }
        }

        public int MaxDistanceSetting
        {
            get
            {
                return GetValueOrDefault<int>(MaxDistanceKeyName, MaxDistanceDefault);
            }
            set
            {
                AddOrUpdateValue(MaxDistanceKeyName, value);
            }
        }

        public int AutoDropPeriodSetting
        {
            get
            {
                return GetValueOrDefault<int>(AutoDropPeriodKeyName, AutoDropPeriodDefault);
            }

            set
            {
                AddOrUpdateValue(AutoDropPeriodKeyName, value);
            }
        }
    }
}
