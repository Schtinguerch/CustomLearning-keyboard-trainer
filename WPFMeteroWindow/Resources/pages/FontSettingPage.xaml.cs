﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFMeteroWindow.Properties;
using System.Text.RegularExpressions;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для FontSettingPage.xaml
    /// </summary>
    public partial class FontSettingPage : Page
    {
        public FontSettingPage()
        {
            InitializeComponent();
            FontTextBox.Text = Settings.Default.FontContext + ' ';
            FontTextBox.Focus();
        }

        private void FontSettingPage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                FontTextBox.Text = Settings.Default.FontContext + ' ';
                FontTextBox.CaretIndex = FontTextBox.Text.Length;
            }

        }

        private void FontSettingPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var fontFamily = Regex.Replace(FontTextBox.Text, "^\\s*(l:|k:)\\s*", "");
                if (FontTextBox.Text.Contains("l:"))
                {
                    Settings.Default.LessonLettersFont = fontFamily;

                    Actions.TheWindow.SetNewLetterFont();
                    Actions.TheWindow.HideSettingGrid();
                }
                else if (FontTextBox.Text.Contains("k:"))
                {
                    Settings.Default.KeyboardFont = fontFamily;
                    
                    Actions.TheWindow.ReloadKeyboard();
                    Actions.TheWindow.HideSettingGrid();
                }

                Settings.Default.Save();
            }
        }
    }
}