using System;
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

namespace Hyaku
{
    public class HyakuSettings : IsolatedAppSettings
    {
        // The isolated storage key names of our settings
        const string GameSizeSettingKeyName = "GameSizeSetting";
        const string EnableTrashRowsKeyName = "EnableTrashRowsSetting";
        const string SweepTimerPeriodKeyName = "SweepTimerPeriodSetting";
        const string TimerTickIntervalKeyName = "TimerTickIntervalSetting";
        const string GameOverFileNameKeyName = "GameOverFileNameSetting";

        // Default settings values
        const int GameSizeSettingDefault = 9;
        const bool EnableTrashRowsDefault = true;
        const int SweepTimerPeriodDefault = 100;
        const int TimerTickIntervalDefault = 300; // milliseconds
        const string GameOverFileNameDefault = "GameOver.dat";

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

        public bool EnableTrashRowsSetting
        {
            get
            {
                return GetValueOrDefault<bool>(EnableTrashRowsKeyName, EnableTrashRowsDefault);
            }
            set
            {
                AddOrUpdateValue(EnableTrashRowsKeyName, value);
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

        public string GameOverFileName
        {
            get
            {
                return GetValueOrDefault(GameOverFileNameKeyName, GameOverFileNameDefault);
            }
            set
            {
                AddOrUpdateValue(GameOverFileNameKeyName, value);
            }
        }
    }
}
