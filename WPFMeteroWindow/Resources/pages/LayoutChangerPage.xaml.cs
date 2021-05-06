using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для LayoutChangerPage.xaml
    /// </summary>
    public partial class LayoutChangerPage : Page
    {
        public LayoutChangerPage()
        {
            InitializeComponent();
            LayoutTextBox.Focus();

            Intermediary.RichPresentManager.Update("Command line", "Choosing a keyboard layout for learning...", "");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) => 
            Opener.NewKeyboardLayoutViaExplorer();

        private void LayoutChangerPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Opener.NewKeyboardLayout(LayoutTextBox.Text);
                PageManager.HidePages();
            }
                
            else if (e.Key == Key.Escape)
                PageManager.HidePages();
        }
    }
}
