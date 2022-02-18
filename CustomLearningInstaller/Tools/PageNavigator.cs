using System;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CustomLearningInstaller
{
    public static class PageNavigator
    {
        private static string _baseFolder = "Resources/Pages";
        private static bool _allowChangePage = true;
        private static int _currentIndex = -1;

        public static bool AllowChangePage
        {
            get => _allowChangePage;
            set
            {
                _allowChangePage = value;

                if (value)
                {
                    GoPreviousButton.IsEnabled = true;
                    GoNextButton.IsEnabled = true;
                }

                else
                {
                    GoPreviousButton.IsEnabled = false;
                    GoNextButton.IsEnabled = false;
                }
            }
        }

        public static Frame Frame { get; set; }
        public static Button GoPreviousButton { get; set; }
        public static Button GoNextButton { get; set; }
        public static List<string> Pages { get; set; } = new List<string>()
        {
            "LicencePage",
            "InstallPathPage",
            "AdditionalsPage",
            "ProcessingPage",
        };

        public static void OpenNextPage()
        {
            if (Frame.CanGoForward)
            {
                Frame.GoForward();
                if (GoPreviousButton != null) GoPreviousButton.IsEnabled = true;
                return;
            }

            if (_currentIndex == Pages.Count - 1)
            {
                if (GoNextButton != null) GoNextButton.IsEnabled = false;
                Common.MainFrame.Source = new Uri($"{_baseFolder}/InstallEndingPage.xaml", UriKind.Relative);
                return;
            }

            if (GoPreviousButton != null) GoPreviousButton.IsEnabled = true;
            Frame.Source = new Uri($"{Pages[++_currentIndex]}.xaml", UriKind.Relative);
        }

        public static void OpenPreviousPage()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                if (GoNextButton != null) GoNextButton.IsEnabled = true;
                return;
            }

            if (GoPreviousButton != null) GoPreviousButton.IsEnabled = false;
        }
    }
}
