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

        // Default settings values
        const int GameSizeSettingDefault = 9;
        const bool EnableTrashRowsDefault = true;
        const int SweepTimerPeriodDefault = 15;

        public int GameSizeSetting
        {
            get
            {
                return GetValueOrDefault<int>(GameSizeSettingKeyName, GameSizeSettingDefault);
            }
            set
            {
                AddOrUpdateValue(GameSizeSettingKeyName, value);
                Save();
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
                AddOrUpdateValue(SweepTimerPeriodKeyName, value);
                Save();
            }
        }
    }
}
