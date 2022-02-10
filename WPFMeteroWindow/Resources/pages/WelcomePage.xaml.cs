using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using WPFMeteroWindow.Resources.pages.WelcomePageSubPages;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        private string _standardSkinPath = "Skins\\Standard.lml";

        private int _currentPageIndex = -1;
        private List<Page> _setupPagesSequence = new List<Page>()
        {
            new KeyboardLayoutSetup(),
            new CourseSetup(),
            new ThemeSetup(),
        };

        private List<string> _setupPagesHeaders = new List<string>()
        {
            Localization.uKeyboadLayout,
            Localization.uSettLessonsAndCourses,
            Localization.uTheme,
            Localization.uSettAnimations,
        };

        public WelcomePage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var storyboard = FindResource("NiceStoryboard") as Storyboard;
                storyboard.Begin();

                var storyboard2 = FindResource("ShowHelloStoryboard") as Storyboard;
                storyboard2.Begin();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            PageManager.HidePages();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var storyboard = FindResource("ShowSetupPanelStoryboard") as Storyboard;
            storyboard.Completed += (s, ee) =>
            {
                BackgroundGrid.Children.Remove(ButtonGrid);
                OpenNextPage();
            };

            storyboard.Begin();
        }

        private void OpenNextPage()
        {
            if (_currentPageIndex == _setupPagesSequence.Count - 1)
            {
                PageManager.HidePages();
                return;
            }

            _currentPageIndex += 1;
            SetupPageFrame.Navigate(_setupPagesSequence[_currentPageIndex]);
            HelloTextBlock.Text = _setupPagesHeaders[_currentPageIndex];
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (_setupPagesSequence[_currentPageIndex] is ThemeSetup)
                UserConfigManager.ImportConfigFromFile(_standardSkinPath);

            OpenNextPage();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var requestable = _setupPagesSequence[_currentPageIndex] as IRequstable;
            if (!requestable.RequestVadid()) return;

            OpenNextPage();
        }
    }
}
