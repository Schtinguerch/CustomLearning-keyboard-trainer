using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WPFMeteroWindow.Properties;

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
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.RestoreDirectory = true;
            openDialog.DefaultExt = "*.lml";

            if (openDialog.ShowDialog() == true)
            {
                Opener.NewKeyboardLayout(openDialog.FileName); 
                PageManager.HidePages();
                
                Settings.Default.Save();
                LayoutTextBox.Text = "";
            }
        }

        private void LayoutChangerPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Opener.NewKeyboardLayout(LayoutTextBox.Text);
                    PageManager.HidePages();
                    
                    Settings.Default.Save();
                    LayoutTextBox.Text = "";
                }

                catch
                {
                    
                }
            }
                
        }

        private void LayoutChangerPage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
