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

namespace Hyaku
{
    public partial class GamePage : PhoneApplicationPage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        SoundEffectInstance bgMusic;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // check if game has control of the media player
            if (MediaPlayer.GameHasControl) {
                // play background music (if enabled in settings menu)

                StreamResourceInfo soundResourceInfo = App.GetResourceStream(new Uri(@"sounds/Themes/blossomTheme/Music.wav", UriKind.Relative));
                SoundEffect bgMusicEffect = SoundEffect.FromStream(soundResourceInfo.Stream);
                bgMusic = bgMusicEffect.CreateInstance();
                bgMusic.IsLooped = true;
                bgMusic.Play();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // turn off the timer
            this.SlickGameBoardView.LeavingPage();
            // stop the music
            if (bgMusic != null) {
                bgMusic.Stop();
            }
            base.OnNavigatedFrom(e);
        }
    }
}