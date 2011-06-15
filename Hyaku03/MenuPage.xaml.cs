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
            App app = Application.Current as App;
            if (app != null) {
                menuPageAdControl.Visibility = app.AdVisibility;
            }
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
    }
}