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
using Hyaku.ViewModels;
using System.ComponentModel;

namespace Hyaku.Views
{
    public partial class SquareView : UserControl
    {
        public event EventHandler SquareClicked;

        private ViewModels.SquareViewModel _square;

        public ViewModels.SquareViewModel Square
        {
            get { return _square; }
            set {
                _square = value;
                this.DataContext = _square;
                _square.PropertyChanged += new PropertyChangedEventHandler(square_PropertyChanged);
                _square.UiSquare = this;
                SetColors();
                SetThickness();
            }
        }

        public SquareView(ViewModels.SquareViewModel square)
        {
            InitializeComponent();
            this.Square = square;
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SquareClicked != null)
            {
                SquareClicked(this, null);
            }
        }

        void square_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentState":
                    SetColors();
                    break;
            }
        }

        private void SetColors()
        {
            switch (_square.CurrentState)
            {
                case SquareState.Hyaku:
                    MainText.Foreground = (SolidColorBrush)Resources["PhoneAccentBrush"];
                    LayoutRoot.Background = new SolidColorBrush(new Color()
                    {
                        R = (byte)(_square.Score / 10)
                    });
                    break;
                case SquareState.Current:
                    MainText.Foreground = (SolidColorBrush)Resources["PhoneAccentBrush"];
                    LayoutRoot.Background = (SolidColorBrush)Resources["PhoneContrastBackgroundBrush"];
                    break;
                case SquareState.Locked:
                default:
                    MainText.Foreground = (SolidColorBrush)Resources["PhoneForegroundBrush"];
                    LayoutRoot.Background = (SolidColorBrush)Resources["PhoneBackgroundBrush"];
                    break;
            }
        }

        private void SetThickness()
        {
            double left = 1;
            double top = 1;
            double right = 1;
            double bottom = 1;
            BoxGridBorder.BorderThickness = new Thickness(left, top, right, bottom);
        }
    }
}
