using System;
using System.Windows;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPFMeteroWindow.Resources.pages
{
    public partial class ApplicationUserSettingsPage : Page
    {
        #region Color Appearance Butons
        private void MainColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(MainColorTextBox);
        
        private void SecondColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(SecondColorTextBox);
        
        private void TextBoxColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(TextBoxColorTextBox);
        
        private void SecondMenuColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(SecondMenuColorTextBox);

        private void LessonSymbolsColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(LessonSymbolsColorTextBox);

        private void UItextColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(UItextColorTextBox);

        private void KeyboardLayoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                DefaultExt = "*.lml"
            };

            if (openDialog.ShowDialog() == true)
            {
                KeyboardLayoutTextBox.Text = openDialog.FileName;
                Settings.Default.KeyboardLayoutFile = openDialog.FileName;
            }
        }

        private void KeyboardButtonsColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(KeyboardButtonsColorTextBox);

        private void KeyboardTextColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(KeyboardTextColorTextBox);

        private void KeyboardFontFamilyButton_OnClick(object sender, RoutedEventArgs e) =>
            SetFontFamily("keyboard");

        private void KeyboardBorderColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(KeyboardBorderColorTextBox);

        private void KeyboardHighLightColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(KeyboardHighLightColorTextBox);

        private void KeyboardErrorHighLightColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(KeyboardErrorHighLightColorTextBox);
        
        private void LessonRaidedSymbolsColorButton_OnClick(object sender, RoutedEventArgs e) =>
            ShowColorPicker(LessonRaidedSymbolsColorTextBox);
        
        private void LessonlettersFontButton_OnClick(object sender, RoutedEventArgs e) =>
            SetFontFamily("main");
        
        private void UIfontButton_OnClick(object sender, RoutedEventArgs e) =>
            SetFontFamily("UI");

        private void HandsColorButton_Click(object sender, RoutedEventArgs e) =>
            ShowColorPicker(HandsColorHighLightColorTextBox);

        #endregion

        #region Color picker (color choosen)

        private void WindowColors_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SetColor.WindowColor(WindowColors.SelectedItem.ToString());
        
        private void MainColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.FirstColor(MainColorTextBox.Text);
        
        private void SecondColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.SecondColor(SecondColorTextBox.Text);
        
        private void TextBoxColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.CommandLineFirstColor(TextBoxColorTextBox.Text);
        
        private void SecondMenuColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.CommandLineSecondColor(SecondMenuColorTextBox.Text);
        
        private void LessonSymbolsColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetFont.MainLetters_Color(LessonSymbolsColorTextBox.Text);
        
        private void UItextColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetFont.Summary_Color(UItextColorTextBox.Text);
        
        private void KeyboardLayoutTextBox_OnTextChanged(object sender, TextChangedEventArgs e) { }
        
        private void KeyboardButtonsColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.KeyboardBackground(KeyboardButtonsColorTextBox.Text);
        
        private void KeyboardTextColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.KeyboardFontColor(KeyboardTextColorTextBox.Text);
        
        private void KeyboardFontFamilyTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetFont.Keyboard(KeyboardFontFamilyTextBox.Text);
        
        private void KeyboardBorderColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.KeyboardBorder(KeyboardBorderColorTextBox.Text);

        private void KeyboardHighLightColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.KeyboardHighlight(KeyboardHighLightColorTextBox.Text);

        private void KeyboardErrorHighLightColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetColor.KeyboardErrorHighlight(KeyboardErrorHighLightColorTextBox.Text);

        private void RequireWPMtextBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SetYesOrNo("WPM", RequireWPMtextBox.SelectedItem.ToString());

        private void ShowStatisticsTextBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SetYesOrNo("Stats", ShowStatisticsTextBox.SelectedItem.ToString());

        private void ShowHandsTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SetYesOrNo("Hands", ShowHandsTextBox.SelectedItem.ToString());

        private void LessonRaidedSymbolsColorTextBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
            SetFont.MainRaidedLetters_Color(LessonRaidedSymbolsColorTextBox.Text);

        private void HandsColorHighLightColorTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
           SetColor.Hands(HandsColorHighLightColorTextBox.Text);

        private void MenuOpacitySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            MenuOpacityTextBox.Text = $"{(e.NewValue * 100).ToString("N")}%";

        private void KeyboardOpacitySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            KeyboardOpacityTextBox.Text = $"{(e.NewValue * 100).ToString("N")}%";

        private void WindowTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (WindowTheme.SelectedIndex)
            {
                case 0:
                    SetColor.ColorScheme(Theme.Light);
                    break;

                case 1:
                    SetColor.ColorScheme(Theme.Dark);
                    break;
            }
        }

        private void WindowColorType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (WindowColorType.SelectedIndex)
            {
                case 0:
                    SetColor.WindowStandardColor();
                    break;

                case 1:
                    SetColor.WindowBackgroundImage(Opener.ImageViaExplorer());
                    break;
            }
        }
        #endregion
    }
}