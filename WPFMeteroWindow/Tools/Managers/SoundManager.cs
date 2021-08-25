using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace WPFMeteroWindow
{
    public static class SoundManager
    {
        private static string _backgroundSoundFile;

        private static MediaPlayer _tapSoundPlayer = new MediaPlayer();
        private static MediaPlayer _errorSoundPlayer = new MediaPlayer();
        private static MediaPlayer _backgroundSoundPlayer = new MediaPlayer();

        public static string TapSoundFile
        {
            get => Settings.Default.TapClickSoundFile;
            set => Settings.Default.TapClickSoundFile = value;
        }

        public static string ErrorSoundFile
        {
            get => Settings.Default.ErrorClickSoundFile;
            set => Settings.Default.ErrorClickSoundFile = value;
            
        }

        public static string BackgroundSoundFile
        {
            get => _backgroundSoundFile;
            set => _backgroundSoundFile = value;
        }

        public static void PlayType()
{
            _tapSoundPlayer.Open(new Uri(Settings.Default.TapClickSoundFile, UriKind.RelativeOrAbsolute));
            _tapSoundPlayer.Play();
        }
            

        public static void PlayTypingMistake()
        {
            _errorSoundPlayer.Open(new Uri(Settings.Default.ErrorClickSoundFile, UriKind.RelativeOrAbsolute));
            _errorSoundPlayer.Play();
        }
            
        public static void PlayBackgroundMusic()
        {
            _backgroundSoundPlayer.Open(new Uri(_backgroundSoundFile, UriKind.RelativeOrAbsolute));
            _backgroundSoundPlayer.Play();
        }
    }
}
