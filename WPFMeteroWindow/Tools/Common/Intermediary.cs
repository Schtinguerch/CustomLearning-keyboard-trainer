﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
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


        public static T GetCopy<T>(this T element) where T : UIElement
        {
            using (var ms = new MemoryStream())
            {
                XamlWriter.Save(element, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)XamlReader.Load(ms);
            }
        }
    }
}