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
using Hyaku.ViewModels;
using System.Windows.Threading;

namespace Hyaku
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        void InputControl_SendInput(object sender, EventArgs e)
        {
            if (sender is int)
            {
                MainBoard.GameBoard.SendNumber((int)sender);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // TODO load from saved game
            GameBoardViewModel board = GameBoardViewModel.CreateNewGame();
            MainBoard.GameBoard = board;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // TODO save game
            base.OnNavigatedFrom(e);
        }
    }
}