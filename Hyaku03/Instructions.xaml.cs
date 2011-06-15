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
        Dictionary<InstructionItem, List<FrameworkElement>> InstructionVisuals;

        public Instructions()
        {
            InitializeComponent();

            InstructionVisuals = new Dictionary<InstructionItem, List<FrameworkElement>>();

            // ProgressBar
            InstructionVisuals.Add(InstructionItem.ProgressBar, new List<FrameworkElement>()
            {
                ProgressBarHighlight,
                ProgressBarDesc
            });

            // GameGrid
            InstructionVisuals.Add(InstructionItem.GameGrid, new List<FrameworkElement>()
            {
                GameGridHighlight,
                GameGridDesc
            });

            // NextNumber
            InstructionVisuals.Add(InstructionItem.NextNumbers, new List<FrameworkElement>()
            {
                NextNumberHighlight,
                QueuedNumberHighlight,
                NextNumberDesc
            });

            // ScoreText
            InstructionVisuals.Add(InstructionItem.Score, new List<FrameworkElement>()
            {
                ScoreHighlight,
                ScoreDesc
            });

            // Hyaku
            InstructionVisuals.Add(InstructionItem.MatchedHyaku, new List<FrameworkElement>()
            {
                HyakuHighlight,
                HyakuDesc
            });
        }

        private void ProgressBarText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(ProgressBarText)) {
                SetVisibleInstructions(InstructionItem.ProgressBar, Visibility.Visible);
                e.Handled = true;
            }
            //if (sender.Equals(ProgressBarText))
            //{
            //    ProgressBarHighlight.Visibility = System.Windows.Visibility.Visible;
            //    ProgressBarDesc.Visibility = System.Windows.Visibility.Visible;
            //    e.Handled = true;
            //}
        }

        private void ProgressBarText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.ProgressBar, System.Windows.Visibility.Collapsed);
            e.Handled = true;
            //if (sender.Equals(ProgressBarText))
            //{
            //    ProgressBarHighlight.Visibility = System.Windows.Visibility.Collapsed;
            //    ProgressBarDesc.Visibility = System.Windows.Visibility.Collapsed;
            //    e.Handled = true;
            //}
        }

        private void GameGridText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.GameGrid, System.Windows.Visibility.Visible);
            e.Handled = true;
            //if (sender.Equals(GameGridText))
            //{
            //    GameGridHighlight.Visibility = System.Windows.Visibility.Visible;
            //    GameGridDesc.Visibility = System.Windows.Visibility.Visible;
            //    e.Handled = true;
            //}
        }

        private void GameGridText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.GameGrid, System.Windows.Visibility.Collapsed);
            e.Handled = true;
            //if (sender.Equals(GameGridText))
            //{
            //    GameGridHighlight.Visibility = System.Windows.Visibility.Collapsed;
            //    GameGridDesc.Visibility = System.Windows.Visibility.Collapsed;
            //    e.Handled = true;
            //}
        }

        private void NextNumberText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.NextNumbers, System.Windows.Visibility.Visible);
            e.Handled = true;
            //if (sender.Equals(NextNumberText))
            //{
            //    NextNumberHighlight.Visibility = System.Windows.Visibility.Visible;
            //    QueuedNumberHighlight.Visibility = System.Windows.Visibility.Visible;
            //    NextNumberDesc.Visibility = System.Windows.Visibility.Visible;
            //    e.Handled = true;
            //}
        }

        private void NextNumberText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.NextNumbers, System.Windows.Visibility.Collapsed);
            e.Handled = true;
            //if (sender.Equals(NextNumberText))
            //{
            //    NextNumberHighlight.Visibility = System.Windows.Visibility.Collapsed;
            //    QueuedNumberHighlight.Visibility = System.Windows.Visibility.Collapsed;
            //    NextNumberDesc.Visibility = System.Windows.Visibility.Collapsed;
            //    e.Handled = true;
            //}
        }

        private void ScoreText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.Score, System.Windows.Visibility.Visible);
            e.Handled = true;
            //if (sender.Equals(ScoreText))
            //{
            //    ScoreHighlight.Visibility = System.Windows.Visibility.Visible;
            //    ScoreDesc.Visibility = System.Windows.Visibility.Visible;
            //}
        }

        private void ScoreText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.Score, System.Windows.Visibility.Collapsed);
            e.Handled = true;
            //if (sender.Equals(ScoreText))
            //{
            //    ScoreHighlight.Visibility = System.Windows.Visibility.Collapsed;
            //    ScoreDesc.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        private void HyakuText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.MatchedHyaku, System.Windows.Visibility.Visible);
            e.Handled = true;
            //if (sender.Equals(HyakuText))
            //{
            //    HyakuHighlight.Visibility = System.Windows.Visibility.Visible;
            //    HyakuDesc.Visibility = System.Windows.Visibility.Visible;
            //}
        }

        private void HyakuText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetVisibleInstructions(InstructionItem.MatchedHyaku, System.Windows.Visibility.Collapsed);
            e.Handled = true;
            //if (sender.Equals(HyakuText))
            //{
            //    HyakuHighlight.Visibility = System.Windows.Visibility.Collapsed;
            //    HyakuDesc.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        public void SetVisibleInstructions(InstructionItem item, Visibility visibilityArg)
        {
            Visibility visibility = System.Windows.Visibility.Collapsed;
            foreach (InstructionItem nextItem in InstructionVisuals.Keys) {

                if (nextItem == item) {
                    // if these are the framework elements for the given item, set visibility according to the argument
                    visibility = visibilityArg;
                } else {
                    // otherwise, collapse the items
                    visibility = System.Windows.Visibility.Collapsed;
                }

                foreach (FrameworkElement element in InstructionVisuals[nextItem]) {
                    element.Visibility = visibility;
                }
            }

        }
    }

    public enum InstructionItem
    {
        None,
        ProgressBar,
        MatchedHyaku,
        GameGrid,
        NextNumbers,
        Score
    }
}
