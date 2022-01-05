using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

using Newtonsoft.Json;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public struct CourseStatistics
    {
        public string CoursePath { get; set; } 
        public List<double> Results { get; set; }
    }

    public static class StatisticsManager
    {
        private const int _millisecondsInMinute = 60000;
        private const int _startTimeToCollectStats = 100;

        private static Stopwatch _typingStopWatch;

        private static int _wordLength = 0;
        private static int _wordTimeStart = 0;
        private static string _currentWord;

        public static List<double> GlobalTypingSpeeds { get; private set; } = 
            AppManager.JsonReadData<List<double>>(Settings.Default.AllTypingSpeedPath);

        public static List<CourseStatistics> CourseStatistics { get; private set; } =
            AppManager.JsonReadData<List<CourseStatistics>>(Settings.Default.CourcesStatisticsPath);

        public static bool IsDemonstrationMode { get; set; } = false;

        public static List<double> AverageSpeeds { get; private set; } = new List<double>();
        public static List<double> WordSpeeds { get; private set; } = new List<double>();
        public static List<string> TimePoints { get; private set; } = new List<string>();
        public static List<int> MistakePoints { get; private set; } = new List<int>();
        public static List<string> MistakeCharacters { get; private set; } = new List<string>();
        public static List<string> MistakeWords { get; private set; } = new List<string>();

        public static int TypingMistakes { get; set; } = 0;
        public static float TypingSpeedCpm { get; private set; } = 0;
        public static int TypingMilliseconds { get; private set; } = 0;
        public static float PassPercentage { get; private set; } = 0;

        public static string TypingTimeOut { get; private set; } = "";
        
        public static Timer TypingTimer { get; private set; }
        
        private static void TickTuck(object sender, EventArgs e)
        {
            TypingMilliseconds = (int)_typingStopWatch.ElapsedMilliseconds;

            var minutes = TypingMilliseconds / _millisecondsInMinute;
            var seconds = TypingMilliseconds / 1000 % 60;
            var milliseconds = TypingMilliseconds % 1000;

            var inputTextLength = LessonManager.DoneRoad.Length -1;
            var averageCpm = inputTextLength / (float)TypingMilliseconds * _millisecondsInMinute;

            if (averageCpm < 0) averageCpm = 0;

            TypingSpeedCpm = averageCpm;
            PassPercentage = (inputTextLength +2) / (float)LessonManager.AllLessonText.Length * 100f;
            TypingTimeOut = $"{minutes:D2}:{seconds:D2}:{milliseconds / 10:D2}";

            if (TypingMilliseconds > _startTimeToCollectStats)
            {
                AverageSpeeds.Add(TypingSpeedCpm);
                TimePoints.Add(TypingTimeOut);
            }

            Intermediary.App.TimerTextBlock.Text = TypingTimeOut;
            Intermediary.App.WPMTextBlock.Text = $"{averageCpm:N} {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = $"{PassPercentage:N}% • {TypingMistakes} {Localization.uMistakes}";
        }

        public static void AddWordStatistics(char inputSymbol)
        {
            if (inputSymbol == ' ' && TypingMilliseconds > _startTimeToCollectStats)
            {
                var wordTime = TypingMilliseconds - _wordTimeStart;
                var wordCpm = (_wordLength + 1) / (float)wordTime * _millisecondsInMinute;

                WordSpeeds.Add(wordCpm);

                try
                {
                    _currentWord = LessonManager.LeftRoad.Substring(0, LessonManager.LeftRoad.IndexOf(' '));
                }

                catch
                {
                    //Do nothing, cuz it's the end of lesson
                }

                _wordLength = 0;
                _wordTimeStart = TypingMilliseconds;
            }
            else
                _wordLength++;
        }

        public static void AddMistakeStatistics(string mistake)
        {
            MistakePoints.Add(TypingMilliseconds);
            MistakeCharacters.Add(mistake);
            MistakeWords.Add(_currentWord);
        }
            
        
        public static void ReloadTimer()
        {
            TypingTimer?.Stop();
            _typingStopWatch?.Stop();
        }

        public static void ReloadStats()
        {
            ReloadTimer();

            AverageSpeeds = new List<double>(12000);
            WordSpeeds = new List<double>(2400);
            
            TimePoints = new List<string>(12000);

            MistakePoints = new List<int>(1200);
            MistakeCharacters = new List<string>(1200);
            MistakeWords = new List<string>(1200);
            
            GC.Collect();

            PassPercentage = 0;
            TypingMistakes = 0;
            TypingSpeedCpm = 0;
            _wordLength = 0;
            _currentWord = LessonManager.LeftRoad.Substring(0, LessonManager.LeftRoad.IndexOf(' '));
        }

        public static void StartTimer()
        {
            ReloadStats();
            TypingTimer = new Timer() { Interval = 20 };
            TypingTimer.Tick += TickTuck;

            _typingStopWatch = Stopwatch.StartNew();
            TypingTimer.Start();
        }
    }
}