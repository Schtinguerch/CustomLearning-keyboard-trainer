using WPFMeteroWindow.Properties;
using LmlLibrary;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System;
using System.Linq;
using System.Windows;

namespace WPFMeteroWindow
{
    public static class LessonManager
    {
        private static string _doneRoad;
        private static string _leftRoad;
        private static string _errorInput;

        public static LessonTextInputPresenter TextInputPresenter { get; set; }

        public static string AllLessonText { get; private set; }

        private static Dictionary<string, string> _exceptions = new Dictionary<string, string>()
        {
            { "\n", " " },
            { "\t", " " },
            { "–", "-" },
            { "—", "-" },
            { "“", "\"" },
            { "”", "\"" },
            { "‘", "'" },
            { "’", "'" },
            { "«", "\"" },
            { "»", "\"" },
            { "…", "..." }
        };

        public static string DoneRoad
        {
            get => _doneRoad;
            set 
            { 
                TextInputPresenter.DoneText = value;
                _doneRoad = value;
            }
        }
        
        public static string LeftRoad
        {
            get => _leftRoad;
            set
            { 
                TextInputPresenter.LeftText = value;
                _leftRoad = value;
            }
        }

        public static string ErrorInput
        {
            get => _errorInput;
            set 
            { 
                TextInputPresenter.ErrorText = value; 
                _errorInput = value;
            }
        }

        public static bool RandomizeText { get; set; } = false;

        public static void LoadLesson(string filename)
        {
            var reader = new Lml(filename, Lml.Open.FromFile);
            string lessonText, lessonName;
            try
            {
                if (Settings.Default.IsCourseOpened)
                    lessonName = Settings.Default.CourseName + ": " + reader.GetString("Lesson>Name");
                else
                    lessonName = reader.GetString("Lesson>Name");
                
                lessonText = Regex.Replace(reader.GetString("Lesson>Text"), "\\s+", " ").ToBeCorrected();
                Settings.Default.NecessaryCPM = reader.GetInt("Lesson>NecessaryCPM");
                Settings.Default.MaxAcceptableMistakes = reader.GetInt("Lesson>MaximumMistakes");
            }
            catch
            {
                lessonName = "...";
                lessonText = File.ReadAllText(filename);
                Settings.Default.NecessaryCPM = -1;
                Settings.Default.MaxAcceptableMistakes = -1;
            }

            StatisticsManager.ReloadStats();

            lessonText = lessonText.Randomized();
            lessonText = lessonText.WithDeletedExceptions();
           
            LoadLessonText(lessonText);

            Intermediary.App.lessonHeaderTextBlock.Text = lessonName;
            
            Settings.Default.LessonName = lessonName;
            Settings.Default.Save();
                
            Intermediary.App.NextLessonButton.Visibility = AppManager.IsVisible(Settings.Default.IsCourseOpened);
            Intermediary.App.PrevLessonButton.Visibility = AppManager.IsVisible(Settings.Default.IsCourseOpened);
            Intermediary.App.TypingProgressIndicator.Width = 0;
            
            KeyboardManager.ShowTypingHint(lessonText[0]);
            
            Intermediary.App.TimerTextBlock.Text = "0:0";
            Intermediary.App.WPMTextBlock.Text = $"0 {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = "0% •  0 " + Localization.uMistakes;

            Settings.Default.ItTypingTest = false;
            Settings.Default.Save();

            Intermediary.App.bufferTextBox.Focus();
        }

        public static void LoadTest(string lessonText)
        {
            StatisticsManager.ReloadStats();
            var text = lessonText.WithDeletedExceptions();

            LoadLessonText(text);
            Intermediary.App.lessonHeaderTextBlock.Text = Localization.uTypingTest;

            Intermediary.App.NextLessonButton.Visibility = Visibility.Hidden;
            Intermediary.App.PrevLessonButton.Visibility = Visibility.Hidden;
            Intermediary.App.TypingProgressIndicator.Width = 0;

            KeyboardManager.ShowTypingHint(LeftRoad[0]);
            
            Intermediary.App.TimerTextBlock.Text = "0:0";
            Intermediary.App.WPMTextBlock.Text = $"0 {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = "0% •  0 " + Localization.uMistakes;

            Settings.Default.ItTypingTest = true;
            Settings.Default.Save();

            Intermediary.App.bufferTextBox.Focus();
        }

        private static void LoadLessonText(string text)
        {
            AllLessonText = text;
            TextInputPresenter.LoadText(text);

            LeftRoad = text;
            DoneRoad = "";
            ErrorInput = "";
        }
        
        public static void EndLesson()
        {            
            StatisticsManager.ReloadTimer();
            PageManager.OpenPage(TabPage.EndedLesson);
        }

        private static string Randomized(this string s)
        {
            if (!RandomizeText) return s;

            var randomizedText = "";
            var wordList = s.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();

            while (wordList.Count > 0)
            {
                var randomizer = new Random(Environment.TickCount);
                var chosenIndex = randomizer.Next(0, wordList.Count);

                randomizedText += wordList[chosenIndex] + ' ';
                wordList.RemoveAt(chosenIndex);
            }

            return randomizedText;
        }

        private static string WithDeletedExceptions(this string s)
        {
            var value = s;

            foreach (var exception in _exceptions)
                value = value.Replace(exception.Key, exception.Value);

            return value;
        }
    }
}