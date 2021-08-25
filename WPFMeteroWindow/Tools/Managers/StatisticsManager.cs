using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public static class StatisticsManager
    {
        private const int _millisecondsInMinute = 60000;

        private static Stopwatch _typingStopWatch;

        private static int _wordLength = 0;
        private static int _wordTimeStart = 0;

        public static List<SpeedPoint> AveragePoints { get; private set; } = new List<SpeedPoint>();
        public static List<SpeedPoint> WordPoinds { get; private set; } = new List<SpeedPoint>();

        public static int TypingMistakes { get; set; } = 0;
        public static float TypingSpeedCpm { get; private set; } = 0;
        public static int TypingMilliseconds { get; private set; } = 0;
        public static float PassPercentage { get; private set; } = 0;
        
        public static Timer TypingTimer { get; private set; }
        
        private static void TickTuck(object sender, EventArgs e)
        {
            TypingMilliseconds = (int) _typingStopWatch.ElapsedMilliseconds;

            var minutes = TypingMilliseconds / _millisecondsInMinute;
            var seconds = TypingMilliseconds / 1000 % 60;
            var milliseconds = TypingMilliseconds % 1000;

            var inputTextLength = LessonManager.DoneRoad.Length;
            var averageCpm = inputTextLength / (float)TypingMilliseconds * _millisecondsInMinute;      

            TypingSpeedCpm = averageCpm;
            PassPercentage = inputTextLength / (float)LessonManager.AllLessonText.Length * 100f;

            if (TypingMilliseconds > 300)
                AveragePoints.Add(new SpeedPoint(TypingMilliseconds, TypingSpeedCpm));

            Intermediary.App.TimerTextBlock.Text = $"{minutes:D2}:{seconds:D2}:{milliseconds/100:D1}";
            Intermediary.App.WPMTextBlock.Text = $"{averageCpm:N} {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = $"{PassPercentage:N}% • {TypingMistakes} {Localization.uMistakes}";
        }

        public static void AddWordStatistics(char inputSymbol)
        {
            if (inputSymbol == ' ')
            {
                var wordTime = TypingMilliseconds - _wordTimeStart;
                var wordCpm = (_wordLength + 1) / (float)wordTime * _millisecondsInMinute;

                WordPoinds.Add(new SpeedPoint(TypingMilliseconds, wordCpm));

                _wordLength = 0;
                _wordTimeStart = TypingMilliseconds;
            }
            else
                _wordLength++;
        }
        
        public static void ReloadTimer()
        {
            TypingTimer?.Stop();
            _typingStopWatch?.Stop();
        }

        public static void ReloadStats()
        {
            ReloadTimer();
            
            AveragePoints = new List<SpeedPoint>();
            WordPoinds = new List<SpeedPoint>();

            PassPercentage = 0;
            TypingMistakes = 0;
            TypingSpeedCpm = 0;
            _wordLength = 0;
        }

        public static void StartTimer()
        {
            ReloadStats();
            TypingTimer = new Timer() { Interval = 25 };
            TypingTimer.Tick += TickTuck;

            _typingStopWatch = Stopwatch.StartNew();
            TypingTimer.Start();
        }
    }
}