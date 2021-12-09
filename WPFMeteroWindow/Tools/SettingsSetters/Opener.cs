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
        public static string ImageViaExplorer()
        {
            var opener = new OpenFileDialog()
            {
                Multiselect = false,
                RestoreDirectory = true,
                Filter = "images|*.jpg;*.png;*.gif;*.ico;*.jpeg",
            };

            if (opener.ShowDialog() == true)
            {
                LogManager.Log($"Open image: \"{opener.FileName}\" -> success");
                return opener.FileName;
            }

            return "";
        }

        public static void NewCourse(string fileName, int lessonIndex, bool skipUiLoading = false)
        {
            if (File.Exists(fileName) || Directory.Exists(fileName))
            {
                Settings.Default.IsCourseOpened = true;
                Settings.Default.LoadedCourseFile = fileName;
                Settings.Default.CourseLessonNumber = lessonIndex;
                Settings.Default.Save();

                try
                {
                    CourseManager.LoadCourse(fileName, skipUiLoading);
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

        public static void NewCourseViaExplorer(bool skipUiLoading = false)
        {
            var openDialog = new FolderBrowserDialog();

            if (openDialog.ShowDialog() == DialogResult.OK)
                NewCourse(openDialog.SelectedPath + "\\CourseLessons.lml", 0, skipUiLoading);
        }
        
        public static void NewKeyboardLayout(string fileName)
        {
            if (File.Exists(fileName))
            {
                if (Settings.Default.CurrentLayout == 1)
                    Settings.Default.KeyboardLayoutFile = fileName;
                else
                    Settings.Default.SecondKeyboardLayoutFile = fileName;

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

        public static void NewKeyboardLayoutViaExplorer(int current)
        {
            var openDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                DefaultExt = "*.lml"
            };

            if (openDialog.ShowDialog() == true)
            {
                Settings.Default.CurrentLayout = current;
                NewKeyboardLayout(openDialog.FileName);
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
                RestoreDirectory = true,
                DefaultExt = ".lml",
                Filter = "LML-files (.lml)|*.lml"
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

        public static void NewTextInputWay(string name)
        {
            switch (name)
            {
                case "Classic":
                case "SingleWord":
                case "SingleLineWithStaticCaret":
                    LessonManager.TextInputPresenter.TextInputControlName = name;
                    Settings.Default.TextInputType = name;
                    break;

                default:
                    LogManager.Log($"Loading text-input presentation -> failed, incorrect name");
                    break;
            }
        }

        public static void Statistics(bool showStastics)
        {
            Intermediary.App.WPMTextBlock.Visibility =
            Intermediary.App.AdditionalInfoPanel.Visibility =
                showStastics ?
                System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            Settings.Default.ShowStatistics = showStastics;
        }
    }
}