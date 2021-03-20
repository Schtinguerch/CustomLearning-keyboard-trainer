using System.IO;
using System.Windows;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public static class Opener
    {
        public static void NewCourse(string fileName, int lessonIndex)
        {
            if (File.Exists(fileName))
            {
                Settings.Default.IsCourseOpened = true;
                Settings.Default.LoadedCourseFile = fileName;
                Settings.Default.CourseLessonNumber = lessonIndex;
                Settings.Default.Save();
                
                CourseManager.LoadCourse(fileName);
            }
            else
            {
                MessageBox.Show("Ошибка: файл, который Вы пытаетесь открыть не существует!");
            }
        }
        
        public static void NewKeyboardLayout(string fileName)
        {
            if (File.Exists(fileName))
            {
                Settings.Default.KeyboardLayoutFile = fileName;
                Settings.Default.Save();
                
                KeyboardManager.LoadKeyboardData(fileName);
            }
            else
            {
                MessageBox.Show("Ошибка: файл, который Вы пытаетесь открыть не существует!");
            }
        }
        
        public static void NewLesson(string fileName)
        {
            if (File.Exists(fileName))
            {
                Settings.Default.IsCourseOpened = false;
                Settings.Default.LoadedLessonFile = fileName;
                Settings.Default.Save();
                
                LessonManager.LoadLesson(fileName);
            }
            else
            {
                MessageBox.Show("Ошибка: файл, который Вы пытаетесь открыть не существует!");
            }
        }
    }
}