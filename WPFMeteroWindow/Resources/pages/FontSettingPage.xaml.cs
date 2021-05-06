using System.Windows;
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

            Intermediary.RichPresentManager.Update("Command line", "Configuring font-families...", "");
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
                    SetFont.MainLetters(fontFamily);
                    PageManager.HidePages();
                }
                else if (FontTextBox.Text.Contains("k:"))
                {
                    SetFont.Keyboard(fontFamily);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    PageManager.HidePages();
                }

                Settings.Default.Save();
            }

            else if (e.Key == Key.Escape)
                PageManager.HidePages();
        }
    }
}
