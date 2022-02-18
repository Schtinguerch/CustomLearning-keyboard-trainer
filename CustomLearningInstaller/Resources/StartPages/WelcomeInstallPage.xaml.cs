using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

using Localization = CustomLearningInstaller.Resources.Localizations.Resources;

namespace CustomLearningInstaller.Resources.StartPages
{
    /// <summary>
    /// Логика взаимодействия для WelcomeInstallPage.xaml
    /// </summary>
    public partial class WelcomeInstallPage : Page
    {
        private Action[] _setLanguageActions = new Action[]
        {
            () => { Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); Common.CultureCode = "en-US"; },
            () => { Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU"); Common.CultureCode = "ru-RU"; },
        };

        public WelcomeInstallPage()
        {
            InitializeComponent();

            switch (ButtonText.Text)
            {
                case "Начать установку":
                    LanguageComboBox.Text = "Русский";
                    break;

                default:
                    LanguageComboBox.Text = "English";
                    break;
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _setLanguageActions[LanguageComboBox.SelectedIndex].Invoke();
            ButtonText.Text = Localization.StartInstallation;
        }

        private void StartInstallationButton_Click(object sender, RoutedEventArgs e)
        {
            Common.MainFrame.Source = new Uri("Resources/Pages/InstallStepsPage.xaml", UriKind.Relative);
        }
    }
}
