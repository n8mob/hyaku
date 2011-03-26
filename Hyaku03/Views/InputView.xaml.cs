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

namespace Hyaku.Views
{
    public partial class InputView : UserControl
    {
        public event EventHandler GetInput;
        public InputView()
        {
            InitializeComponent();
        }

        public void UserInput_Click(object sender, RoutedEventArgs e)
        {
            int inputValue = int.Parse(((Button)sender).Tag.ToString());
            if (GetInput != null)
            {
                GetInput(inputValue, null);
            }
        }

        public void RotateVertical()
        {
        }

        public void RotateHorizontal()
        {
        }
    }
}
