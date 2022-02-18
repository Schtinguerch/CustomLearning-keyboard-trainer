using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomLearningInstaller.Resources.Pages
{
    /// <summary>
    /// Логика взаимодействия для InstallEndingPage.xaml
    /// </summary>
    public partial class InstallEndingPage : Page
    {
        public InstallEndingPage()
        {
            InitializeComponent();
        }

        private void EndInstallationButton_Click(object sender, RoutedEventArgs e)
        {
            if (LaunchAppCheckBox.IsChecked == true)
                System.Diagnostics.Process.Start(AppInstaller.ApplicationFilename);

            if (VisitTrainerPageCheckBox.IsChecked == true)
                System.Diagnostics.Process.Start("https://github.com/Schtinguerch/CustomLearning-keyboard-trainer");

            Application.Current.Shutdown();
        } 
    }
}
