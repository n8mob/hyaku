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
using Microsoft.Phone.Tasks;
using Microsoft.Advertising.Mobile.UI;

namespace Hyaku
{
    public partial class MenuPage : PhoneApplicationPage
    {
        App OurApp
        {
            get
            {
                return Application.Current as App;
            }
        }

        public MenuPage()
        {
            InitializeComponent();
            if (OurApp != null) {
                menuPageAdControl.Visibility = OurApp.TrialItemVisibility;
                ContinueGameMenuItem.Visibility = OurApp.FullVersionVisibility;
                FullVersionMenuItem.Visibility = OurApp.TrialItemVisibility;
            }
#if DEBUG
            SettingsMenuItem.Visibility = System.Windows.Visibility.Visible;
            AdControl.TestMode = true;
#else
            SettingsMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            AdControl.TestMode = false;
#endif
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

        private void FeedbackButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask feedbackMail = new Microsoft.Phone.Tasks.EmailComposeTask();
            feedbackMail.To = "hyaku@nategrigg.com";
            feedbackMail.Subject = "Game Feedback";
            feedbackMail.Show();
        }

        private void FullVersionMenuItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show(Messages.FullVersionMessage, Messages.FullVersionCaption, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK) {
                MarketplaceDetailTask marketplace = new MarketplaceDetailTask();
                marketplace.Show();
            }
        }
    }
}