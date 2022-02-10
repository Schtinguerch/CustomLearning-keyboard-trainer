using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages.WelcomePageSubPages
{
    /// <summary>
    /// Логика взаимодействия для KeyboardLayoutSetup.xaml
    /// </summary>
    public partial class KeyboardLayoutSetup : Page, IRequstable
    {
        private List<string> _keyboardLayoutPaths = new List<string>();

        public KeyboardLayoutSetup()
        {
            InitializeComponent();
            LoadKeyboardLayouts();
        }

        public bool RequestVadid()
        {
            if (KeyboardLayoutComboBox.SelectedItem == null)
            {
                KeyboardLayoutComboBox.BorderBrush = Brushes.Red;
                return false;
            }

            Opener.NewKeyboardLayout(_keyboardLayoutPaths[KeyboardLayoutComboBox.SelectedIndex]);
            Settings.Default.Save();

            KeyboardLayoutComboBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 18, 18, 18));
            return true;
        }

        private void LoadKeyboardLayouts()
        {
            var folder = "KeyboardLayouts";
            _keyboardLayoutPaths = Directory.GetFiles(folder, "*.lml").ToList();

            foreach (var path in _keyboardLayoutPaths)
            {
                var header = path.Split('\\').Last().Replace(".lml", "");
                KeyboardLayoutComboBox.Items.Add(header);
            }

            KeyboardLayoutComboBox.SelectedIndex = 6;
        }

        private void KeyboardLayoutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var path = _keyboardLayoutPaths[KeyboardLayoutComboBox.SelectedIndex];
            KeyboardGrid.LoadButtons(path);
        }
    }
}
