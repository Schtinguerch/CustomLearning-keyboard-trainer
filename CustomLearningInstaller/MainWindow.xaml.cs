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
using MahApps.Metro.Controls;

namespace CustomLearningInstaller
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Common.MainWindow = this;
            Common.MainFrame = MainFrame;

            if (Environment.GetCommandLineArgs().Last() != "/uninstall")
            {
                MainFrame.Source = new Uri("Resources/StartPages/WelcomeInstallPage.xaml", UriKind.Relative);
            }
            else
            {
                MainFrame.Source = new Uri("Resources/StartPages/UninstallPage.xaml", UriKind.Relative);
            }
        }
    }
}
