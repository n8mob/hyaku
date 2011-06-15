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
    public partial class Instructions : PhoneApplicationPage
    {
        public Instructions()
        {
            InitializeComponent();
        }

        private void ProgressBarText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(ProgressBarText))
            {
                ProgressBarHighlight.Visibility = System.Windows.Visibility.Visible;
                ProgressBarDesc.Visibility = System.Windows.Visibility.Visible;
                e.Handled = true;
            }
        }

        private void ProgressBarText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(ProgressBarText))
            {
                ProgressBarHighlight.Visibility = System.Windows.Visibility.Collapsed;
                ProgressBarDesc.Visibility = System.Windows.Visibility.Collapsed;
                e.Handled = true;
            }
        }

        private void GameGridText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(GameGridText))
            {
                GameGridHighlight.Visibility = System.Windows.Visibility.Visible;
				GameGridDesc.Visibility = System.Windows.Visibility.Visible;
                e.Handled = true;
            }
        }

        private void GameGridText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(GameGridText))
            {
                GameGridHighlight.Visibility = System.Windows.Visibility.Collapsed;
				GameGridDesc.Visibility = System.Windows.Visibility.Collapsed;
                e.Handled = true;
            }
        }

        private void NextNumberText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(NextNumberText))
            {
                NextNumberHighlight.Visibility = System.Windows.Visibility.Visible;
				QueuedNumberHighlight.Visibility = System.Windows.Visibility.Visible;
				NextNumberDesc.Visibility = System.Windows.Visibility.Visible;
                e.Handled = true;
            }
        }

        private void NextNumberText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(NextNumberText))
            {
                NextNumberHighlight.Visibility = System.Windows.Visibility.Collapsed;
				QueuedNumberHighlight.Visibility = System.Windows.Visibility.Collapsed;
				NextNumberDesc.Visibility = System.Windows.Visibility.Collapsed;
                e.Handled = true;
            }
        }

        private void ScoreText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(ScoreText))
            {
                ScoreHighlight.Visibility = System.Windows.Visibility.Visible;
				ScoreDesc.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ScoreText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(ScoreText))
            {
                ScoreHighlight.Visibility = System.Windows.Visibility.Collapsed;
				ScoreDesc.Visibility = System.Windows.Visibility.Collapsed;
            }
        
        }

        private void HyakuText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(HyakuText))
            {
                HyakuHighlight.Visibility = System.Windows.Visibility.Visible;
				HyakuDesc.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void HyakuText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(HyakuText))
            {
                HyakuHighlight.Visibility = System.Windows.Visibility.Collapsed;
				HyakuDesc.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}
