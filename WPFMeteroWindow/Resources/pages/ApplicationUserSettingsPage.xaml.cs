using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using WPFMeteroWindow.Properties;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;
using TextBox = System.Windows.Controls.TextBox;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ApplicationUserSettingsPage.xaml
    /// </summary>
    public partial class ApplicationUserSettingsPage : Page
    {
        private UIElement _chosenTextBox;

        private readonly List<string> _windowColors = new List<string>()
        {
            "amber",
            "blue",
            "brown",
            "cobalt",
            "crimson",
            "cyan",
            "emerald",
            "green",
            "indigo",
            "lime",
            "magenta",
            "mauve",
            "olive",
            "orange",
            "pink",
            "purple",
            "red",
            "sienna",
            "steel",
            "taupe",
            "teal",
            "violet",
            "yellow"
        };

        private void SetYesOrNo(string target, string text)
        {
            var state = true;
            var selectedText = text.ToLower();

            if ((selectedText == "no") || (selectedText == "нет"))
                state = false;

            switch (target)
            {
                case "WPM":
                    Settings.Default.RequireWPM = state;
                    break;

                case "Stats":
                    Settings.Default.ShowStatistics = state;
                    break;

                case "Hands":
                    Settings.Default.ShowHands = state;
                    break;
            }
        }

        private void SetFontFamily(string target)
        {
            var fontDialog = new FontDialog();
            fontDialog.ShowColor = false;
            fontDialog.ShowEffects = false;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                var converter = new FontConverter();
                var fontFamily = converter.ConvertToString(fontDialog.Font).Split(new char[] {';'})[0];

                switch (target)
                {
                    case "keyboard":
                        SetFont.Keyboard(fontFamily);
                        break;

                    case "main":
                        SetFont.MainLetters(fontFamily);
                        break;

                    case "UI":
                        SetFont.SummaryLetters(fontFamily);
                        break;
                }
            }
        }
        
        private void ShowColorPicker(UIElement targetControl)
        {
            _chosenTextBox = targetControl;

            var relativePoint = targetControl.TransformToAncestor(wrapPanel).Transform(new Point(0, 0));
            SetupColorPicker.Margin = 
                new Thickness(-206, relativePoint.Y + targetControl.RenderSize.Height, 0, 0);

            SetupColorPicker.IsOpen = true;
        }
        
        public ApplicationUserSettingsPage()
        {
            InitializeComponent();
            
            WindowColors.Items.Clear();
            foreach (var color in _windowColors)
                WindowColors.Items.Add(color);
        }

        private void SetupColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ((TextBox) _chosenTextBox).Text = SetupColorPicker.SelectedColorText;
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
            PageManager.HidePages();
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();
            KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
            PageManager.HidePages();
        }

        private void LessonlettersFontSizeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var fontsize = Convert.ToDouble(LessonlettersFontSizeTextBox.Text);
                SetFont.MainLetters_Size(fontsize);
            }
            
            catch
            {
                
            }
        }      
    }
}
