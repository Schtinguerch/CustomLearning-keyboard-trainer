using System;
using System.Windows;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public enum TabPage
    {
        UserSettings = 0,
        CommandLine,
        CourseLoaderShell,
        LessonLoaderShell,
        LayoutLoaderShell,
        FontSetterShell,
        EndedLesson,
        LessonEditor,
        CourseEditor,
        EmptyPage,
    }
    
    public static class PageManager
    {
        private static string _baseFolder = "Resources/pages/";

        public static Frame PageFrame { get; set; }
        
        public static Grid PageGrid { get; set; }
        
        public static string[] Pages =
        {
            _baseFolder + "ApplicationUserSettingsPage.xaml",
            _baseFolder + "CommandLinePage.xaml",
            _baseFolder + "CourseLoaderPage.xaml",
            _baseFolder + "LessonLoaderPage.xaml",
            _baseFolder + "LayoutChangerPage.xaml",
            _baseFolder + "FontSettingPage.xaml",
            _baseFolder + "LessonIsEndPage.xaml",
            _baseFolder + "LessonEditorPage.xaml",
            _baseFolder + "CourseEditorPage.xaml",
            null,
        };
        
        public static void HidePages()
        {
            PageGrid.Visibility = Visibility.Hidden;
            PageFrame.Source = null;

            Intermediary.RichPresentManager.Update(
                Settings.Default.ItTypingTest ? "Typing test" : Settings.Default.LessonName, "Chilling...",
                "");
        }

        public static void OpenPage(TabPage page)
        {
            int pageIndex = (int) page;
            if (pageIndex >= 0)
            {
                PageGrid.Visibility = Visibility.Visible;
                PageFrame.Source = new Uri(Pages[pageIndex], UriKind.Relative);
            }
            else if (pageIndex == Pages.Length - 1)
                HidePages();
        }
    }
}