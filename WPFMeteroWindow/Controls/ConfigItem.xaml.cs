using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using LmlLibrary;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для ConfigItem.xaml
    /// </summary>
    public partial class ConfigItem : UserControl
    {
        private string _configFullPath;
        private bool _hasImage = false;

        public string MainBackgroundColor { get; set; }
        public string SecondBackgroundColor { get; set; }
        public string SkinFontColor { get; set; }
        public string SkinFontFamily { get; set; }
        public string ImagePath { get; set; }
        public double BlurRadius { get; set; }

        public ConfigItem(string skinPath)
        {
            InitializeConfig(skinPath);
            InitializeComponent();
            AfterInitializing();
        }

        private void InitializeConfig(string skinPath)
        {
            _configFullPath = skinPath;
            var reader = new Lml(skinPath, Lml.Open.FromFile);

            MainBackgroundColor = reader.GetString("UserConfig>AppColors>MainColor");
            SecondBackgroundColor = reader.GetString("UserConfig>AppColors>SecondaryColor");

            SkinFontColor = reader.GetString("UserConfig>Fonts>AppGUIFontColor");
            SkinFontFamily = reader.GetString("UserConfig>Fonts>AppGUIFontFamily");

            _hasImage = reader.GetBool("UserConfig>AppColors>HasImageBackground");

            if (_hasImage)
            {
                ImagePath = reader.GetString("UserConfig>Wallpaper>PathToImage");
                
                var numberString = reader.GetString("UserConfig>Wallpaper>BlurRadius").Replace(",", ".");
                BlurRadius = double.Parse(numberString, CultureInfo.InvariantCulture);
            }
        }

        private void AfterInitializing()
        {
            _configNameTextBlock.Text = _configFullPath.Split('\\').Last();
            const int maxNameLength = 52;

            if (_configFullPath.Length >= maxNameLength)
            {
                int length = maxNameLength;
                int startIndex = _configFullPath.Length - length;

                _configPathTextBlock.Text = "..." + _configFullPath.Substring(startIndex, length);
            }
            else
            {
                _configPathTextBlock.Text = _configFullPath;
            }

            if (_hasImage)
            {
                _backgroundImage.ImageSource  = new BitmapImage(new Uri(ImagePath));
                _imageBlurEffect.Radius = BlurRadius;
            }
        }

        private void ItemBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _shadowEffect.BlurRadius = 32;
            ItemBorder.CornerRadius = new CornerRadius(20);
        }

        private void ItemBorder_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserConfigManager.ImportConfigFromFile(_configFullPath);

            _shadowEffect.BlurRadius = 24;
            ItemBorder.CornerRadius = new CornerRadius(16);
        }

        private void ItemBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _shadowEffect.BlurRadius = 24;
            ItemBorder.CornerRadius = new CornerRadius(16);
        }


        private void ItemBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _shadowEffect.BlurRadius = 16;
            ItemBorder.CornerRadius = new CornerRadius(10);
        }
    }
}
