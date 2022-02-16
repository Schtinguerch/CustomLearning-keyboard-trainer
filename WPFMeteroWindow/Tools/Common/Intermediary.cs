using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Properties;
using WPFMeteroWindow.Resources.pages;

namespace WPFMeteroWindow
{
    public static class Intermediary
    {
        public static MainWindow App { get; set; }

        public static CourseEditorPage CoursePage { get; set; }

        public static WordPracticingEditorPage WordPracticingPage { get; set; }

        public static LessonEditorPage LessonPage { get; set; }

        public static KeyboardLayoutEditorPage LayoutPage { get; set; }
        
        public static DiscordManager RichPresentManager { get; set; }

        public static AllSplashShapes AllSplashShapes { get; set; }
        public static Dictionary<string, Grid> KeyboardShapesDictionary { get; set; }
        public static List<string> KeyboardStoryboards { get; set; }

        public static MouseSplashShapes MouseSplashShapes { get; set; }
        public static Dictionary<string, Grid> MouseShapesDictionary { get; set; }
        public static List<string> MouseStoryboards { get; set; }
        
        public static (string[][] keys, Button[] buttons) KeyboardData { get; set; }

        public static List<string> RecentImages = AppManager.JsonReadData<List<string>>(Settings.Default.RecentPhotosPath);

        public static int KeyboardCharIndex { get; set; }
        
        public static int KeyboardModifierIndex { get; set; }
    }
}