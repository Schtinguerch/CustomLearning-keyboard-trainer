﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
        LessonEditor,
        CourseEditor,
        KeyboardLayoutEditor,
        RecentCources,
        RecentLayouts,
        RecentConfigs,
        TypingTestParameters,
        WelcomePage,
        ShortCutHint,
        LessonCompleted,
        EmptyPage,
    }
    
    public static class PageManager
    {
        private static string _baseFolder = "Resources/pages/";
        private static TabPage _tabPage = TabPage.EmptyPage;

        public static TabPage CurrentPage => _tabPage;
        public static Frame PageFrame { get; set; }
        public static Grid PageGrid { get; set; }

        public static Storyboard OpenPageStoryboard { get; set; }
        public static Storyboard ClosePageStoryboard { get; set; }

        public static string[] Pages =
        {
            _baseFolder + "ApplicationUserSettingsPage.xaml",
            _baseFolder + "CommandLinePage.xaml",
            _baseFolder + "CourseLoaderPage.xaml",
            _baseFolder + "LessonLoaderPage.xaml",
            _baseFolder + "LayoutChangerPage.xaml",
            _baseFolder + "FontSettingPage.xaml",
            _baseFolder + "LessonEditorPage.xaml",
            _baseFolder + "CourseEditorPage.xaml",
            _baseFolder + "KeyboardLayoutEditorPage.xaml",
            _baseFolder + "ActiveUsingLessonsPage.xaml",
            _baseFolder + "ActiveUsingLayoutsPage.xaml",
            _baseFolder + "ActiveUsingConfigs.xaml",
            _baseFolder + "TypingTestParametersPage.xaml",
            _baseFolder + "WelcomePage.xaml",
            _baseFolder + "ShortcutHintPage.xaml",
            _baseFolder + "LessonCompletedPage.xaml",
            null,
        };

        public static void HidePages()
        {
            _tabPage = TabPage.EmptyPage;
            if (ClosePageStoryboard == null)
            {
                ClearPages();
                return;
            }

            ClosePageStoryboard.Completed += StoryboardCompleted;
            ClosePageStoryboard.Begin();
        }

        private static void ClearPages()
        {
            PageGrid.Visibility = Visibility.Hidden;
            PageFrame.Source = null;

            Intermediary.RichPresentManager.Update(
                Settings.Default.ItTypingTest ? "Typing test" : Settings.Default.LessonName, "Chilling...",
                "");

            GC.Collect();
        }

        public static void OpenPage(TabPage page, bool usingAnimation = true)
        {
            _tabPage = page;
            int pageIndex = (int)page;

            if (pageIndex == Pages.Length - 1)
            {
                HidePages();
            } 

            else if (pageIndex >= 0)
            {
                PageGrid.Visibility = Visibility.Visible;
                PageFrame.Source = new Uri(Pages[pageIndex], UriKind.Relative);

                if (OpenPageStoryboard == null) return;

                if (usingAnimation)
                    OpenPageStoryboard.Children[0].Duration = new TimeSpan(0, 0, 0, 0, 500);
                else
                    OpenPageStoryboard.Children[0].Duration = new TimeSpan(0);

                OpenPageStoryboard.Begin();
            }
        }

        public static void MakeTrasparency()  => PageGrid.Opacity = 0.1d;
        public static void CancelTransparency() => PageGrid.Opacity = 1d;

        private static void StoryboardCompleted(object sender, EventArgs e)
        {
            ClearPages();
            ClosePageStoryboard.Completed -= StoryboardCompleted;
        }
    }
}