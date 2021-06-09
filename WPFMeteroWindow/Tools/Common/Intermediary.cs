using System.Windows.Controls;
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
        
        public static (string[][] keys, Button[] buttons) KeyboardData { get; set; }
        
        public static int KeyboardCharIndex { get; set; }
        
        public static int KeyboardModifierIndex { get; set; }
    }
}