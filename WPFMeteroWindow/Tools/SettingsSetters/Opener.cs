using System;
using System.IO;
using System.Windows.Forms;
using WPFMeteroWindow.Properties;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

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

                try
                {
                    CourseManager.LoadCourse(fileName);
                    LogManager.Log($"Open course: \"{fileName}\" -> success");
                }

                catch (Exception e)
                {
                    MessageBox.Show(Localization.uOpenDataWithErrorsError);
                    LogManager.Log($"Open course: \"{fileName}\" -> failed, {e.Message}");
                }
                
            }
            else
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Open course: \"{fileName}\" -> failed, file does not exist");
            }
        }

        public static void NewCourseViaExplorer()
        {
            var openDialog = new FolderBrowserDialog();

            if (openDialog.ShowDialog() == DialogResult.OK)
                NewCourse(openDialog.SelectedPath + "\\CourseLessons.lml", 0);
        }
        
        public static void NewKeyboardLayout(string fileName)
        {
            if (File.Exists(fileName))
            {
                Settings.Default.KeyboardLayoutFile = fileName;
                Settings.Default.Save();

                try
                {
                    KeyboardManager.LoadKeyboardData(fileName);
                    LogManager.Log($"Open keyboard layout: \"{fileName}\" -> success");
                }

                catch (Exception e)
                {
                    MessageBox.Show(Localization.uOpenDataWithErrorsError);
                    LogManager.Log($"Open keyboard layout \"{fileName}\" -> failed, {e.Message}");
                }
                
            }
            else
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Open keyboard layout: \"{fileName}\" -> failed, file does not exist");
            }
        }
        
        public static void NewLesson(string fileName)
        {
            if (File.Exists(fileName))
            {
                Settings.Default.IsCourseOpened = false;
                Settings.Default.LoadedLessonFile = fileName;
                Settings.Default.ItTypingTest = false;
                 
                Settings.Default.Save();

                try
                {
                    LessonManager.LoadLesson(fileName);
                    LogManager.Log($"Open course: \"{fileName}\" -> success");
                }

                catch (Exception e)
                {
                    MessageBox.Show(Localization.uOpenDataWithErrorsError);
                    LogManager.Log($"Open lesson: \"{fileName}\" -> failed, {e.Message}");
                }
            }
            else
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Open lesson: \"{fileName}\" -> failed, file does not exist");
            }
        }

        public static void NewLessonViaExplorer()
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                RestoreDirectory = true
            };

            if (openDialog.ShowDialog() == true)
            {
                NewLesson(openDialog.FileName);
                PageManager.HidePages();
            }
        }

        public static void NewTest(TestWords testWords, TestAdditional additional)
        {
            Settings.Default.ItTypingTest = true;
            Settings.Default.Save();

            try
            {
                TestManager.StartTest(testWords, additional);
                LogManager.Log($"Starting test -> success");
            }
            catch
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Starting test -> failed, file does not exist");
            }
        }

        public static void NewTest(int first, int last, TestAdditional additional)
        {
            Settings.Default.ItTypingTest = true;
            Settings.Default.Save();

            try
            {
                TestManager.StartTest(first, last, additional);
                LogManager.Log($"Starting test -> success");
            }
            catch
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Starting test -> failed, file does not exist");
            }
        }
    }
}