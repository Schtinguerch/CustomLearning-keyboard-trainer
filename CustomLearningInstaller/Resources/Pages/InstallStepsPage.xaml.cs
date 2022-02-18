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
    /// Логика взаимодействия для InstallStepsPage.xaml
    /// </summary>
    public partial class InstallStepsPage : Page
    {
        public InstallStepsPage()
        {
            InitializeComponent();
            PageNavigator.Frame = StepsFrame;
            Common.BottomTextBlock = BottomMessageTextBlock;

            PageNavigator.GoNextButton = GoNextButton;
            PageNavigator.OpenNextPage();
        }

        private void GoNextButton_Click(object sender, RoutedEventArgs e)
        {
            PageNavigator.OpenNextPage();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            PageNavigator.OpenPreviousPage();
        }
    }
}
