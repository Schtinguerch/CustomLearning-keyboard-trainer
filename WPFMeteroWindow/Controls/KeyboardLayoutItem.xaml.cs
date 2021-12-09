using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFMeteroWindow.Properties;
using LmlLibrary;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для KeyboardLayoutItem.xaml
    /// </summary>
    public partial class KeyboardLayoutItem : UserControl
    {
        private string _layoutFullPath;

        public string LayoutFilename
        {
            get => _layoutNameTextBlock.Text;
        }

        public string FirstSixCharacters
        {
            get => _firstLettersTextBlock.Text;
        }

        public string LayoutPath
        {
            get => _layoutFullPath;
            set
            {
                const int maxNameLength = 52;
                _layoutFullPath = value;

                if (_layoutFullPath.Length >= maxNameLength)
                {
                    int length = maxNameLength;
                    int startIndex = _layoutFullPath.Length - length;

                    _layoutPathTextBlock.Text = "..." + _layoutFullPath.Substring(startIndex, length);
                }
                else
                {
                    _layoutPathTextBlock.Text = _layoutFullPath;
                }

                var reader = new Lml(value, Lml.Open.FromFile);
                var firstSixCharacters = "";

                for (int i = 0; i < 6; i++)
                    firstSixCharacters += reader.GetArray($"Layout>k{i + 15}")[0].ToBeCorrected().ToUpper();

                _firstLettersTextBlock.Text = firstSixCharacters;
                _layoutNameTextBlock.Text = value.Split('\\').Last();
            }
        }

        public KeyboardLayoutItem()
        {
            InitializeComponent();
        }

        private void ItemBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.ThirdBackground);
            ItemBorder.CornerRadius = new CornerRadius(20);
        }

        private void ItemBorder_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Opener.NewKeyboardLayout(_layoutFullPath);

            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.MainBackground);
            ItemBorder.CornerRadius = new CornerRadius(16);
        }

        private void ItemBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.SecondBackground);
            ItemBorder.CornerRadius = new CornerRadius(16);
        }


        private void ItemBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.MainBackground);
            ItemBorder.CornerRadius = new CornerRadius(10);
        }
    }
}
