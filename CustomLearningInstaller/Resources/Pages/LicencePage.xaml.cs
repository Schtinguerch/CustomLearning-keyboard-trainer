using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace CustomLearningInstaller.Resources.Pages
{
    /// <summary>
    /// Логика взаимодействия для LicencePage.xaml
    /// </summary>
    public partial class LicencePage : Page
    {
        private string _baseFolder = "https://raw.githubusercontent.com/Schtinguerch/schtinguerch.github.io/master/Agreements/";

        public LicencePage()
        {
            InitializeComponent();
            var licencePath = $"{_baseFolder}/Licence-en-US.txt";

            var client = new WebClient();
            TermsTextBox.Text = client.DownloadString(licencePath);
        }
    }
}
