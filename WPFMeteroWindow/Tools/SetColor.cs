﻿using System;
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
        }

        public static void SecondColor(string color)
        {
            Settings.Default.SecondBackground = color;
        }

        public static void CommandLineFirstColor(string color)
        {
            Settings.Default.ThirdBackground = color;
        }

        public static void CommandLineSecondColor(string color)
        {
            Settings.Default.ThirdSecBackground = color;
        }

        public static void KeyboardBackground(string color)
        {
            Settings.Default.KeyboardBackgroundColor = color;
        }

        public static void KeyboardFontColor(string color)
        {
            Settings.Default.KeyboardFontColor = color;
        }
        

        public static void KeyboardBorder(string color)
        {
            Settings.Default.KeyboardBorderColor = color;
        }

        public static void KeyboardHighlight(string color)
        {
            Settings.Default.KeyboardHighlightColor = color;
        }

        public static void KeyboardErrorHighlight(string color)
        {
            Settings.Default.KeyboardErrorHighlightColor = color;
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