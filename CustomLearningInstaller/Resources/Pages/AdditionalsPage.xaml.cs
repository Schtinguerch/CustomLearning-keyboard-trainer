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

namespace CustomLearningInstaller.Resources.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdditionalsPage.xaml
    /// </summary>
    public partial class AdditionalsPage : Page
    {
        public AdditionalsPage()
        {
            InitializeComponent();
            Common.BottomTextBlock.Text = "";

            CreateDesktopCheckBox.IsChecked = true;
            CreateStartCheckBox.IsChecked = true;
        }

        private void CreateDesktopCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AppInstaller.CreateDesktopShortcut = (bool) CreateDesktopCheckBox.IsChecked;
        }

        private void CreateStartCheckBox_Click(object sender, RoutedEventArgs e)
        {
            AppInstaller.AddShortcutToStart = (bool) CreateStartCheckBox.IsChecked;
        }
    }
}
