using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для LessonLoaderPage.xaml
    /// </summary>
    public partial class LessonLoaderPage : Page
    {
        public LessonLoaderPage()
        {
            InitializeComponent();
            LessonTextBox.Focus();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                RestoreDirectory = true
            };

            if (openDialog.ShowDialog() == true)
            {
                Opener.NewLesson(openDialog.FileName);
                Actions.TheWindow.HideSettingGrid();
            }
        }

        private void LessonLoaderPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Opener.NewLesson(LessonTextBox.Text);
                Actions.TheWindow.HideSettingGrid();
            }
        }
    }
}
