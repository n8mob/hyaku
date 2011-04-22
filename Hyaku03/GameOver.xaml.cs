﻿using System;
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
using NateGrigg.Mobile.Utility;

namespace Hyaku
{
    public partial class GameOver : PhoneApplicationPage
    {
        public GameOver()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            HyakuSettings settings = new HyakuSettings();
            string gameOverMessage = IsolatedStorageHandler.ReadUtf8String(settings.GameOverFileName);
            GameOverMessageTextBlock.Text = gameOverMessage;
        }
    }
}