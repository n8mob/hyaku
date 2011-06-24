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
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Content;
using Hyaku.ViewModels;

namespace Hyaku
{
    public partial class GamePage : PhoneApplicationPage
    {
        SoundEffectInstance bgMusic;
        string currentTheme;

        public GamePage()
        {
            InitializeComponent();
            SlickGameControl.GameEnded += new GameOverEventHandler(GameOverHandler);
        }

        private void GameOverHandler(object sender, GameOverEventArgs e)
        {
            // TODO handle game-over
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            HyakuSettings hyakuSettings = new HyakuSettings();

            // set the theme
            if (this.NavigationContext.QueryString.ContainsKey("theme")) {
                currentTheme = this.NavigationContext.QueryString["theme"];
                SlickGameControl.CurrentTheme = currentTheme;
            } else {
                SlickGameControl.CurrentTheme = "blossomTheme"; // TODO make a setting?
            }

            string continueParameter;
            bool continueSavedGame = false;
            if (this.NavigationContext.QueryString.ContainsKey("continue")) {
                continueParameter = this.NavigationContext.QueryString["continue"];
                bool.TryParse(continueParameter, out continueSavedGame);
            }
            string savedGame = hyakuSettings.SavedGame;

            if (continueSavedGame && !string.IsNullOrEmpty(savedGame)) {
                // load saved game (if any)
                SlickGameControl.GameBoard = new GameBoardViewModel(); //GameBoardViewModel.LoadGameFromString(savedGame);
                SlickGameControl.GameBoard.LoadGameFromString(savedGame);
            } else {
                // no saved game, or use asked for new game
                SlickGameControl.GameBoard = new GameBoardViewModel(); //GameBoardViewModel.CreateNewGame();
                SlickGameControl.GameBoard.CreateNewGame();
            }

            // check if game has control of the media player
            if (MediaPlayer.GameHasControl) {
                if (!MediaPlayer.IsMuted) {
                    // play background music (if enabled in settings menu)
                    if (hyakuSettings.PlayBackgroudnMusicSetting) {
                        string uriString = @"sounds/Themes/" + currentTheme + @"/Music.wav";
                        StreamResourceInfo soundResourceInfo = App.GetResourceStream(new Uri(uriString, UriKind.Relative));
                        SoundEffect bgMusicEffect = SoundEffect.FromStream(soundResourceInfo.Stream);
                        bgMusic = bgMusicEffect.CreateInstance();
                        bgMusic.IsLooped = true;
                        bgMusic.Play();
                    }
                }
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // turn off the timer
            this.SlickGameControl.LeavingPage();
            // stop the music
            if (bgMusic != null) {
                bgMusic.Stop();
            }

            base.OnNavigatedFrom(e);
        }
    }
}