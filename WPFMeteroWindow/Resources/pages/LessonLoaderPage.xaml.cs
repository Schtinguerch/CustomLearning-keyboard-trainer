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

            Intermediary.RichPresentManager.Update("Command line", "Choosing a lesson for passing", "");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) =>
            Opener.NewLessonViaExplorer();

        private void LessonLoaderPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Opener.NewLesson(LessonTextBox.Text);
                PageManager.HidePages();
            }

            else if (e.Key == Key.Escape)
                PageManager.HidePages();
        }
    }
}
