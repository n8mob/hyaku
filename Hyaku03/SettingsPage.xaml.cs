using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Hyaku
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void CurrentSweepTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void UseDebugNumbersSetting_Click(object sender, RoutedEventArgs e)
        {
            DebugNumbersTextBox.IsEnabled = UseDebugNumbersSetting.IsChecked ?? false;
        }
    }
}