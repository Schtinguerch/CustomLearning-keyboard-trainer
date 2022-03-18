using System;
using System.Collections.Generic;
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
            if (!File.Exists(path) && (path.Substring(0, 4) != "http"))
            {
                if (!string.IsNullOrEmpty(path))
                    LogManager.Log($"Set background image: \"{path}\" -> failed, file does not exist");

                return;
            }

            if (!path.Contains("http") && !path.Contains(":\\"))
            {
                path = $"{AppDomain.CurrentDomain.BaseDirectory}\\{path}";
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

            if (Intermediary.RecentImages == null)
            {
                Intermediary.RecentImages = new List<string>();
            }

            if (!Intermediary.RecentImages.Contains(path))
            {
                Intermediary.RecentImages.Add(path);
            }
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

        public static void AccentColor(string color)
        {
            if (color.IsValidColor())
                Settings.Default.HighlightColor = color;
        }

        public static void BordersColor(string color)
        {
            if (color.IsValidColor())
                Settings.Default.BordersColor = color;
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
    }
}