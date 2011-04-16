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
using NateGrigg.Mobile.Utility;
using System.IO;
using System.IO.IsolatedStorage;

namespace Hyaku
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string savedGameFileName = "hyaku.dat";
        const string settingsPageUri = "/SettingsPage.xaml";
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
            GameBoardViewModel board = null;
            string savedGame;
            try {
                savedGame = IsolatedStorageHandler.ReadUtf8String(savedGameFileName);
                // if there is no file, ReadUtf8String will throw a FileNotFoundException and the message box will not appear.
            } catch (IsolatedStorageException) {
                savedGame = string.Empty;
            } catch (FileNotFoundException) {
                savedGame = string.Empty;
            }
            
            if (!string.IsNullOrEmpty(savedGame)) {
                board = GameBoardViewModel.LoadGameFromString(savedGame);
            } else {
                board = GameBoardViewModel.CreateNewGame();
            }
            board.GameOver += new GameOverEventHandler(GameOverHandler);
            MainBoard.GameBoard = board;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            
            string gameState;
            try {
                if (!e.Uri.ToString().Equals("/GameOver.xaml")) {
                    gameState = MainBoard.GameBoard.ToString();
                    IsolatedStorageHandler.WriteUtf8String(savedGameFileName, gameState);
                } else {
                    gameState = string.Empty;
                    IsolatedStorageHandler.WriteUtf8String(savedGameFileName, string.Empty);
                    this.MainBoard.GameBoard = null;
                }
            } catch {
                MessageBox.Show(ErrorMessages.SaveFailed);
            }

            base.OnNavigatingFrom(e);
        }

        public void GameOverHandler(object sender, GameOverEventArgs e)
        {
            string gameOverMessage = Messages.GameOverCaption;
            if (e.Reason == GameOverReason.RanOutOfSpace) {
                gameOverMessage = Messages.RanOutOfSpaceMessage;
            } else if (e.Reason == GameOverReason.PushedPastTop) { 
                gameOverMessage = Messages.PushedPastTopMessage;
            }

            //MainBoard.GameBoard = null;

            NavigationService.Navigate(new Uri("/GameOver.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(settingsPageUri, UriKind.Relative));
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult resetGame = MessageBox.Show(Messages.ResetGameQuestion, Messages.ResetGameCaption, MessageBoxButton.OKCancel);
            if (resetGame == MessageBoxResult.OK) {
                MainBoard.GameBoard = GameBoardViewModel.CreateNewGame();
            }
        }
    }
}