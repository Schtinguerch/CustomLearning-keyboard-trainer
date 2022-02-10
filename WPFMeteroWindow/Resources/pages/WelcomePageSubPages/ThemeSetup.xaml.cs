using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages.WelcomePageSubPages
{
    /// <summary>
    /// Логика взаимодействия для ThemeSetup.xaml
    /// </summary>
    public partial class ThemeSetup : Page, IRequstable
    {
        private List<string> _skinPaths = new List<string>();

        public ThemeSetup()
        {
            InitializeComponent();
            LoadSkins();

            MouseLightingDonut.RepeatForever = true;
            KeyboardLightingDonut.RepeatForever = true;

            MouseLightingDonut.Shape = Intermediary.MouseShapesDictionary[Settings.Default.ChosenClickSplashName];
            KeyboardLightingDonut.Shape = Intermediary.KeyboardShapesDictionary[Settings.Default.ChosenSplashShapeName];

            if (Settings.Default.IsBackgroundImage)
            {
                BackgroundImage.Opacity = 1d;
                BackgroundImage.Source = new BitmapImage(new Uri(Settings.Default.BackgroundImagePath));
            }
            else
                BackgroundImage.Opacity = 0d;
        }

        public bool RequestVadid()
        {
            if (SkinComboBox.SelectedItem == null)
            {
                SkinComboBox.BorderBrush = Brushes.Red;
                return false;
            }

            UserConfigManager.ImportConfigFromFile(_skinPaths[SkinComboBox.SelectedIndex]);
            SkinComboBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 18, 18, 18));

            return true;
        }

        private void LessonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserConfigManager.ImportConfigFromFile(_skinPaths[SkinComboBox.SelectedIndex]);

            if (Settings.Default.IsBackgroundImage)
            {
                BackgroundImage.Opacity = 1d;
                BackgroundImage.Source = new BitmapImage(new Uri(Settings.Default.BackgroundImagePath));
            }
            else
                BackgroundImage.Opacity = 0d;
        }

        private void LoadSkins()
        {
            var folder = "Skins";
            _skinPaths = Directory.GetFiles(folder, "*.lml").ToList();

            foreach (var skin in _skinPaths)
            {
                var header = skin.Split('\\').Last().Replace("*.lml", "");
                SkinComboBox.Items.Add(header);
            }
        }
    }
}
