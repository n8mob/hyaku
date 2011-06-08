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
            App app = Application.Current as App;
            string gameOverMessage = Messages.UnknownGameOverMessage;
            if (app != null) {
                switch (app.LastGameOver) {
                    case GameOverReason.PushedPastTop:
                        gameOverMessage = Messages.PushedPastTopMessage;
                        break;
                    case GameOverReason.RanOutOfSpace:
                        gameOverMessage = Messages.RanOutOfSpaceMessage;
                        break;
                    case GameOverReason.LessThanZero:
                        gameOverMessage = Messages.LessThanZeroMessage;
                        break;
                    case GameOverReason.MoreThanMax:
                        gameOverMessage = Messages.MoreThanMaxMessage;
                        break;
                    default:
                        gameOverMessage = Messages.UnknownGameOverMessage;
                        break;
                }
            }
            GameOverMessageTextBlock.Text = gameOverMessage;
        }
    }
}