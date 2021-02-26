using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ApplicationUserSettingsPage.xaml
    /// </summary>
    public partial class ApplicationUserSettingsPage : Page
    {
        private UIElement _chosenTextBox;

        private void ShowColorPicker()
        {
            var relativePoint = _chosenTextBox.TransformToAncestor(wrapPanel).Transform(new Point(0, 0));
            SetupColorPicker.Margin = new Thickness(-206, relativePoint.Y + _chosenTextBox.RenderSize.Height, 0, 0);

            SetupColorPicker.IsOpen = true;
        }
        
        public ApplicationUserSettingsPage()
        {
            InitializeComponent();
        }

        private void WindowColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void MainColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = MainColorTextBox;
            ShowColorPicker();
        }

        private void SecondColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = SecondColorTextBox;
            ShowColorPicker();
        }

        private void TextBoxColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = TextBoxColorTextBox;
            ShowColorPicker();
        }

        private void SecondMenuColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = SecondMenuColorTextBox;
            ShowColorPicker();
        }

        private void LessonSymbolsColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = LessonSymbolsColorTextBox;
            ShowColorPicker();
        }

        private void UItextColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = UItextColorTextBox;
            ShowColorPicker();
        }

        private void KeyboardLayoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void KeyboardButtonsColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = KeyboardButtonsColorTextBox;
            ShowColorPicker();
        }

        private void KeyboardTextColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = KeyboardTextColorTextBox;
            ShowColorPicker();
        }

        private void KeyboardFontFamilyButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void KeyboardBorderColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = KeyboardBorderColorTextBox;
            ShowColorPicker();
        }

        private void KeyboardHighLightColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = KeyboardHighLightColorTextBox;
            ShowColorPicker();
        }

        private void KeyboardErrorHighLightColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _chosenTextBox = KeyboardErrorHighLightColorTextBox;
            ShowColorPicker();
        }

        private void RequireWPMbutton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ShowStatisticsButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
