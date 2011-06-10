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
using Microsoft.Xna.Framework.Media;

namespace Hyaku
{
    public partial class MenuPage : PhoneApplicationPage
    {
        public MenuPage()
        {
            InitializeComponent();
            
            if (IsInTrialMode()) {
                MessageBox.Show("Trial Mode");
            }
        }

        private bool IsInTrialMode()
        {
#if DEBUG
            return true;
#endif
            Microsoft.Phone.Marketplace.LicenseInformation license = new Microsoft.Phone.Marketplace.LicenseInformation();
            return license.IsTrial();
        }

        private void oldUiButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void newUiButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}