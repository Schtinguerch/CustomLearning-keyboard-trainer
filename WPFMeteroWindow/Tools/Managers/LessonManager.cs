﻿using WPFMeteroWindow.Properties;
using LmlLibrary;
using System.IO;
using System.Text.RegularExpressions;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Threading;

namespace WPFMeteroWindow
{
    public static class LessonManager
    {
        public static string DoneRoad { get; set; }
        
        public static int StoppedDoneRoad { get; set; }
        
        public static string LeftRoad { get; set; }

        public static bool RandomizeText { get; set; } = false;

        public static void LoadLesson(string filename)
        {
            Intermediary.App.errorInputTextBlock.Text = "";
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
                lessonText = File.ReadAllText(filename).Replace("\n", " ").Replace("\t", " ");
                Settings.Default.NecessaryCPM = -1;
                Settings.Default.MaxAcceptableMistakes = -1;
            }

            DoneRoad = "";
            Intermediary.App.inputTextBox.Text = "";
            StatisticsManager.ReloadStats();

            lessonText = lessonText.Randomized();
            LeftRoad = lessonText;

            Intermediary.App.inputTextBlock.Text = lessonText;
            Intermediary.App.lessonHeaderTextBlock.Text = lessonName;
            
            Settings.Default.LessonName = lessonName;
            Settings.Default.Save();
                
            Intermediary.App.NextLessonButton.Visibility = AppManager.IsVisible(Settings.Default.IsCourseOpened);
            Intermediary.App.PrevLessonButton.Visibility = AppManager.IsVisible(Settings.Default.IsCourseOpened);
            
            KeyboardManager.ShowTypingHint(LeftRoad[0]);
            
            Intermediary.App.TimerTextBlock.Text = "0:0";
            Intermediary.App.WPMTextBlock.Text = $"0 {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = "0 " + Localization.uMistakes;

            Settings.Default.ItTypingTest = false;
            Settings.Default.Save();
        }

        public static void LoadTest(string lessonText)
        {
            DoneRoad = "";
            Intermediary.App.inputTextBox.Text = "";
            StatisticsManager.ReloadStats();

            LeftRoad = lessonText;
            Intermediary.App.inputTextBlock.Text = lessonText;
            Intermediary.App.lessonHeaderTextBlock.Text = Localization.uTypingTest;

            Intermediary.App.NextLessonButton.Visibility = Visibility.Hidden;
            Intermediary.App.PrevLessonButton.Visibility = Visibility.Hidden;
            
            KeyboardManager.ShowTypingHint(LeftRoad[0]);
            
            Intermediary.App.TimerTextBlock.Text = "0:0";
            Intermediary.App.WPMTextBlock.Text = $"0 {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = "0 " + Localization.uMistakes;

            Settings.Default.ItTypingTest = true;
            Settings.Default.Save();
        }
        
        public static void EndLesson()
        {
            Settings.Default.TypingTime = StatisticsManager.TypingMilliseconds;
            Settings.Default.TypingLength = DoneRoad.Length;
            Settings.Default.TypingErrors = StatisticsManager.TypingErrors;
            Settings.Default.Save();
            
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
    }
}