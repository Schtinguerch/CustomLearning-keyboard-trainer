using System;
using System.Windows;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public enum Theme
    {
        Light,
        Dark
    }
    public static class SetColor
    {
        public static void FirstColor(string color)
        {
            Settings.Default.MainBackground = color;
            Settings.Default.Save();
        }

        public static void SecondColor(string color)
        {
            Settings.Default.SecondBackground = color;
            Settings.Default.Save();
        }

        public static void CommandLineFirstColor(string color)
        {
            Settings.Default.ThirdBackground = color;
            Settings.Default.Save();
        }

        public static void CommandLineSecondColor(string color)
        {
            Settings.Default.ThirdSecBackground = color;
            Settings.Default.Save();
        }

        public static void KeyboardBackground(string color)
        {
            Settings.Default.KeyboardBackgroundColor = color;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }

        public static void KeyboardBorder(string color)
        {
            Settings.Default.KeyboardBorderColor = color;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }

        public static void KeyboardHighlight(string color)
        {
            Settings.Default.KeyboardHighlightColor = color;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
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
            }
            
            Settings.Default.ColorSchemeResourceDictionary = baseFolder + color + ".xaml";
            Settings.Default.Save();
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
            Settings.Default.Save();
        }
    }
}