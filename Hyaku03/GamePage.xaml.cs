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
            string savedGame = hyakuSettings.SavedGame;
            App app = Application.Current as App;
            if (app != null) {
                if (app.ContinueFromSaved && !string.IsNullOrEmpty(savedGame)) {
                    // load saved game (if any)
                    SlickGameControl.GameBoard = GameBoardViewModel.LoadGameFromString(savedGame);
                } else {
                    // no saved game, or use asked for new game
                    SlickGameControl.GameBoard = GameBoardViewModel.CreateNewGame();
                }
            }
            if (string.IsNullOrEmpty(hyakuSettings.SavedGame)) {
                // start new game
            } else {
                // restore saved game
            }
            // check if game has control of the media player
            if (MediaPlayer.GameHasControl) {
                if (!MediaPlayer.IsMuted) {
                    // play background music (if enabled in settings menu)
                    if (hyakuSettings.PlayBackgroudnMusicSetting) {
                        StreamResourceInfo soundResourceInfo = App.GetResourceStream(new Uri(@"sounds/Themes/blossomTheme/Music.wav", UriKind.Relative));
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