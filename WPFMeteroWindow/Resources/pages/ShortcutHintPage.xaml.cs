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

using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ShortcutHintPage.xaml
    /// </summary>
    public partial class ShortcutHintPage : Page
    {
        public ShortcutHintPage()
        {
            InitializeComponent();

            if (Settings.Default.ItTypingTest)
                TypingTestHintTextBox.Text = Localization.uBackToLessons;
        }
    }
}
