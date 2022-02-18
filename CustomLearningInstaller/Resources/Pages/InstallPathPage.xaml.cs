using System;
using System.Collections.Generic;
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

using Localization = CustomLearningInstaller.Resources.Localizations.Resources;

namespace CustomLearningInstaller.Resources.Pages
{
    /// <summary>
    /// Логика взаимодействия для InstallPathPage.xaml
    /// </summary>
    public partial class InstallPathPage : Page
    {
        public InstallPathPage()
        {
            InitializeComponent();

            Common.BottomTextBlock.Text = Localization.AtLeast;
            FolderPathTextBox.Text = FolderPathTextBox.Text;
        }

        private void FolderPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppInstaller.DestinationFolder = FolderPathTextBox.Text;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = FolderPathTextBox.Text;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AppInstaller.DestinationFolder = dialog.SelectedPath;
            }
        }
    }
}
