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
    public partial class GamePage : PhoneApplicationPage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // turn off the timer
            this.SlickGameBoardView.LeavingPage();
            base.OnNavigatedFrom(e);
        }
    }
}