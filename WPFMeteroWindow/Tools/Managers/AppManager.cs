using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WPFMeteroWindow.Properties;
using System.Collections.Generic;
using System.Windows.Controls;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System.Windows.Media;

namespace WPFMeteroWindow
{
    public static class AppManager
    {
        public static void FindMainWindow()
        {
            foreach (Window window in Application.Current.Windows)
                if (window.GetType() == typeof(MainWindow))
                {
                    Intermediary.App = window as MainWindow;
                    break;
                }
        }

        public static void InitializeApplication()
        {
            FindMainWindow();
            ParallaxEffectPresenter.Image = Intermediary.App.BackgroundImage;
            ParallaxEffectPresenter.Init();

            if (!Settings.Default.ShowHands)
            {
                Intermediary.App.LeftHandFrame.Visibility = Visibility.Hidden;
                Intermediary.App.RightHandFrame.Visibility = Visibility.Hidden;
            }

            if (Settings.Default.IsBackgroundImage)
            {
                if (File.Exists(Settings.Default.BackgroundImagePath))
                    SetColor.WindowBackgroundImage(Settings.Default.BackgroundImagePath);
                else 
                    SetColor.WindowStandardColor();
            }
            
            int errorIndex = -1;

            try
            {
                errorIndex++;
                Opener.NewKeyboardLayout(Settings.Default.KeyboardLayoutFile);

                errorIndex++;
                if (Settings.Default.IsCourseOpened)
                {
                    errorIndex++;
                    CourseManager.LoadCourse(Settings.Default.LoadedCourseFile);
                }
                else
                    LessonManager.LoadLesson(Settings.Default.LoadedLessonFile);
            }
            catch
            {
                var isNecessaryFileExists = true;

                switch (errorIndex)
                {
                    case 0:
                        isNecessaryFileExists = File.Exists(Settings.Default.KeyboardLayoutFile);
                        Opener.NewKeyboardLayout("DefaultData\\SchtDvorak.lml");
                        break;

                    case 1:
                        isNecessaryFileExists = File.Exists(Settings.Default.LoadedLessonFile);
                        Opener.NewLesson("DefaultData\\Monday.lml");
                        break;

                    case 2:
                        isNecessaryFileExists = File.Exists(Settings.Default.LoadedCourseFile);
                        Opener.NewLesson("DefaultData\\Monday.lml");
                        break;
                }

                MessageBox.Show(
                    Localization.uErrorCannotOpenFile +
                    (isNecessaryFileExists ? Localization.uErrorAccessDenied : Localization.uErrorFileNotFound) + "\n\n" +
                    Localization.uSystemReplaceProblemFileIntoStandard);

                MessageBox.Show(
                    Localization.uErrorCode + $": {errorIndex.ToString()}\n" +
                    $"{Localization.uKeyboadLayout}: \"{Settings.Default.KeyboardLayoutFile}\"\n" +
                    $"{Localization.uLesson}: \"{Settings.Default.LoadedLessonFile}\"\n" +
                    $"{Localization.uCourse}: \"{Settings.Default.LoadedCourseFile}\"");
            }
        }
        
        public static string ToBeCorrected(this string s)
        {
            var val = s;

            val = val.Replace(";nth;", "");
            val = val.Replace(";ap;", "'");
            val = val.Replace(";qt;", "\"");
            val = val.Replace(";opbr;", "{");
            val = val.Replace(";clbr;", "}");
            val = val.Replace(";space;", " ");

            return val;
        }

        public static string ToLmlFormat(this string s)
        {
            var val = string.IsNullOrEmpty(s) ? ";nth;" : s;

            val = val.Replace("'", ";ap;");
            val = val.Replace("\"", ";qt;");
            val = val.Replace("{", ";opbr;");
            val = val.Replace("}", ";clbr;");

            return val;
        }

        public static bool IsInRange(this int number, int a, int b) => ((number >= a) && (number <= b));

        public static bool IsEqualOr(this int number, params int[] numbers)
        {
            var value = false;

            foreach (var num in numbers)
            {
                if (num == number)
                {
                    value = true;
                    break;
                }
            }

            return value;
        }

        private static Color _checkColor;

        private static double _checkNumber;

        public static bool IsValidColor(this string color)
        {
            try
            {
                _checkColor = (Color)ColorConverter.ConvertFromString(color);
                return true;
            }

            catch
            {
                MessageBox.Show(Localization.uUnknownColorError);
                LogManager.Log($"Set color \"{color}\" -> failed, invalid color name");

                return false;
            }
        }

        public static bool IsValidNumber(this string number)
        {
            try
            {
                _checkNumber = Convert.ToDouble(number);
                return true;
            }

            catch
            {
                MessageBox.Show(Localization.uIncorrectNumberError);
                LogManager.Log($"Set number \"{number}\" -> failed, incorrect value");

                return false;
            }
        }

        public static void SelectLine(this TextBox textBox)
        {
            int charIndex = textBox.CaretIndex;
            int startPoint = charIndex;

            if (textBox.CaretIndex == textBox.Text.Length - 1)
                startPoint -= 3;

            while ((textBox.Text[startPoint] != '\n') && (startPoint > 0))
                startPoint--;

            if ((startPoint != 0) && (startPoint < textBox.Text.Length))
                startPoint++;

            int
                endPoint = startPoint + 1,
                selectLength = 0;

            try
            {
                while ((textBox.Text[endPoint] != '\n') && (endPoint < textBox.Text.Length))
                {
                    endPoint++;
                    selectLength++;
                }

                textBox.Select(startPoint, selectLength + 1);
            }

            catch
            {
                textBox.Select(startPoint, textBox.Text.Length - startPoint);
            }
        }

        public static string KeyListToString(this List<Key> keys)
        {
            var value = "";
            for (int i = 0; i < keys.Count - 1; i++)
                value += keys[i].ToString() + " + ";

            if (keys.Count > 0)
                value += keys[keys.Count - 1].ToString();

            return value;
        }

        public static string ArgsToString(this object[] args, int startIndex)
        {
            var value = "";

            if (args.Length != 0)
            {
                value += args[startIndex + 0].ToString();

                for (int i = startIndex + 1; i < args.Length; i++)
                    value += ' ' + args[i].ToString();
            }

            return value;
        }

        public static string StartPath(string basePath)
        {
            return basePath.Contains(":\\") ? basePath : System.AppDomain.CurrentDomain.BaseDirectory + $"\\{basePath}";
        }

        public static bool IsComboKeyDown(KeyEventArgs e, params Key[] keys)
        {
            bool isKeyDown = true;

            foreach (var key in keys)
            {
                if (!(e.KeyboardDevice.IsKeyDown(key)))
                {
                    isKeyDown = false;
                    break;
                }
            }

            return isKeyDown;
        }

        public static Visibility IsVisible(bool visible)
        {
            if (visible)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public static List<string> GetFileList(string[] badArray)
        {
            var rightFileList = new List<string>();

            var listItem = "";
            foreach (var item in badArray)
            {
                if (!item.Contains(".lml"))
                    listItem += item + ' ';
                else
                {
                    rightFileList.Add(listItem + item);
                    listItem = "";
                }
            }

            return rightFileList;
        }
    }
}
