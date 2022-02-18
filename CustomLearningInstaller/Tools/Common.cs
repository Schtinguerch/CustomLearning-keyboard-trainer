using System;
using System.Windows.Controls;

namespace CustomLearningInstaller
{
    public static class Common
    {
        public static MainWindow MainWindow { get; set; }
        public static Frame MainFrame { get; set; }
        public static TextBlock BottomTextBlock { get; set; }

        public static string CultureCode { get; set; } = "en-US";
    }
}
