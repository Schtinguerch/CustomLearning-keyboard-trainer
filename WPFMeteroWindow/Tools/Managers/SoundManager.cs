using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;
using System.Windows.Media;

namespace WPFMeteroWindow
{
    public static class SoundManager
    {
        private static string _backgroundSoundFile;
        private static MediaPlayer _player = new MediaPlayer();

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

        public static string ClickSoundFile
        {
            get => Settings.Default.MouseClickSoundFile;
            set => Settings.Default.MouseClickSoundFile = value;
        }

        public static string BackgroundSoundFile
        {
            get => _backgroundSoundFile;
            set => _backgroundSoundFile = value;
        }

        public static double TypingVolume
        {
            get => Settings.Default.TypingVolume;
            set => Settings.Default.TypingVolume = value;
        }

        public static double ClickVolume
        {
            get => Settings.Default.ClickVolume;
            set => Settings.Default.ClickVolume = value;
        }

        public static double BackgroundSoundVolume
        {
            get => Settings.Default.BackgroundMusicVolume;
            set => Settings.Default.BackgroundMusicVolume = value;
        }

        public static void Initialize()
        {
            TypingVolume = TypingVolume;
            TapSoundFile = TapSoundFile;
            ErrorSoundFile = ErrorSoundFile;
        }

        public static void PlayType()
{
            _player.Volume = TypingVolume;
            _player.Open(new Uri(TapSoundFile, UriKind.RelativeOrAbsolute));
            _player.Play();
        }
        
        public static void PlayTypingMistake()
        {
            _player.Volume = TypingVolume;
            _player.Open(new Uri(ErrorSoundFile, UriKind.RelativeOrAbsolute));
            _player.Play();
        }
            
        public static void PlayBackgroundMusic()
        {
            _player.Volume = BackgroundSoundVolume;
            _player.Open(new Uri(BackgroundSoundFile, UriKind.RelativeOrAbsolute));
            _player.Play();
        }

        public static void PlayClick()
        {
            _player.Volume = ClickVolume;
            _player.Open(new Uri(ClickSoundFile, UriKind.RelativeOrAbsolute));
            _player.Play();
        }
    }
}
