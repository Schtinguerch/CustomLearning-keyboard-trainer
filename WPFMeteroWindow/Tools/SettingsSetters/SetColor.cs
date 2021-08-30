using System;
using System.Windows;
using WPFMeteroWindow.Properties;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System.Windows.Media.Animation;

namespace WPFMeteroWindow
{
    public enum Theme
    {
        Light,
        Dark
    }

    public static class SetColor
    {
        
        public static void WindowBackgroundImage(string path)
        {
            if (!File.Exists(path))
            {
                if (!string.IsNullOrEmpty(path))
                    LogManager.Log($"Set background image: \"{path}\" -> failed, file does not exist");

                return;
            }

            Settings.Default.IsBackgroundImage = true;
            Settings.Default.BackgroundImagePath = path;


            Intermediary.App.BackgroundImage.Visibility = Visibility.Visible;
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.EndInit();

            ImageBehavior.SetAnimatedSource(Intermediary.App.BackgroundImage, image);
            ImageBehavior.SetRepeatBehavior(Intermediary.App.BackgroundImage, RepeatBehavior.Forever);
        }

        public static void WindowStandardColor()
        {
            Intermediary.App.BackgroundImage.Visibility = Visibility.Hidden;
            ImageBehavior.SetAnimatedSource(Intermediary.App.BackgroundImage, null);

            Settings.Default.IsBackgroundImage = false;
            Settings.Default.BackgroundImagePath = "";
        }

        public static void FirstColor(string color)
        {
            if (color.IsValidColor())
            {
                Settings.Default.MainBackground = color;

                if (!Settings.Default.IsBackgroundImage)
                    Intermediary.App.MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
        }


        public static void SecondColor(string color)
        {
            if (color.IsValidColor())
                Settings.Default.SecondBackground = color;
        }

        public static void CommandLineFirstColor(string color)
        {
            if (color.IsValidColor())
                Settings.Default.ThirdBackground = color;
        }

        public static void CommandLineSecondColor(string color)
        {
            if (color.IsValidColor())
                Settings.Default.ThirdSecBackground = color;
        }

        public static void KeyboardBackground(string color)
        {
            if (color.IsValidColor())
                Settings.Default.KeyboardBackgroundColor = color;
        }

        public static void KeyboardFontColor(string color)
        {
            if (color.IsValidColor())
                Settings.Default.KeyboardFontColor = color;
        }

        public static void KeyboardBorder(string color)
        {
            if (color.IsValidColor())
                Settings.Default.KeyboardBorderColor = color;
        }

        public static void KeyboardHighlight(string color)
        {
            if (color.IsValidColor())
                Settings.Default.KeyboardHighlightColor = color;
        }

        public static void KeyboardErrorHighlight(string color)
        {
            if (color.IsValidColor())
                Settings.Default.KeyboardErrorHighlightColor = color;
        }

        public static void Hands(string color)
        {
            if (color.IsValidColor())
                Settings.Default.HandsColor = color;
        }

        public static void HandsThickness(string thickness)
        {
            if (thickness.IsValidNumber())
                Settings.Default.HandsThickness = thickness;
        }
            

        public static void WindowColor(string color)
        {
            var appDictionaries = Application.Current.Resources.MergedDictionaries;
            var baseFolder = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/";
            
            try
            {
                appDictionaries[appDictionaries.Count - 3] = new ResourceDictionary()
                {
                    Source = new Uri(
                        baseFolder + color + ".xaml", UriKind.RelativeOrAbsolute)
                };
            }
            catch
            {
                appDictionaries[appDictionaries.Count - 3] = new ResourceDictionary()
                {
                    Source = new Uri(baseFolder + "steel.xaml", UriKind.RelativeOrAbsolute)
                };

                color = "steel";
                LogManager.Log("Set window title color -> failed, invalid theme name");
            }
            
            Settings.Default.ColorSchemeResourceDictionary = baseFolder + color + ".xaml";
        }

        public static void ColorScheme(string theme)
        {
            if (theme.ToLower() == "light")
                ColorScheme(Theme.Light);
            else 
                ColorScheme(Theme.Dark);
        }

        public static void ColorScheme(Theme theme)
        {
            var themeFile = "";
            var appDictionaries = Application.Current.Resources.MergedDictionaries;
            
            switch (theme)
            {
                case Theme.Light:
                    themeFile = "BaseLight.xaml";
                    break;
                
                case Theme.Dark:
                    themeFile = "BaseDark.xaml";
                    break;
                
                default:
                    return;
            }

            var uri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + themeFile;
            appDictionaries[appDictionaries.Count - 2] = new ResourceDictionary()
            {
                Source = new Uri(uri, UriKind.RelativeOrAbsolute)
            };
            
            Settings.Default.ThemeResourceDictionary = uri;
        }
    }
}