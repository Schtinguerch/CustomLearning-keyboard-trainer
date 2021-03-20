using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Forms;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public static class StatisticsManager
    {
        public static List<SpeedPoint> ChartPoints { get; set; } = new List<SpeedPoint>();
        
        public static List<SpeedPoint> AveragePoints { get; set; } = new List<SpeedPoint>();

        public static int TypingErrors { get; set; } = 0;

        public static int TypingSpeed { get; set; } = 0;

        public static int TypingMilliseconds { get; set; } = 0;
        
        public static Timer TypingTimer { get; private set; }
        
        private static void TickTuck(object sender, EventArgs e)
        {
            TypingMilliseconds++;
            Intermediary.App.TimerTextBlock.Text = TypingMilliseconds / 10 + ":" + TypingMilliseconds % 10;

            if (TypingMilliseconds % 10 == 0)
            {
                ChartPoints.Add(new SpeedPoint(TypingMilliseconds / 10 - 1, LessonManager.DoneRoad.Length - LessonManager.StoppedDoneRoad));
                AveragePoints.Add(new SpeedPoint(TypingMilliseconds / 10 -1, (float) LessonManager.DoneRoad.Length / TypingMilliseconds * 10));
                LessonManager.StoppedDoneRoad = LessonManager.DoneRoad.Length;
            }

            Intermediary.App.WPMTextBlock.Text = 
                (LessonManager.DoneRoad.Length / (double)TypingMilliseconds * 10d * 60).ToString("N") + $" {Localization.uCPM}";
            Intermediary.App.MistakesTextBloxck.Text = TypingErrors.ToString() + ' ' + Localization.uMistakes;
        }
        
        public static void ReloadTimer()
        {
            if (TypingTimer != null)
                TypingTimer.Stop();
            
            TypingTimer = new Timer();
            TypingTimer.Interval = 94;
            TypingTimer.Tick += TickTuck;

            TypingMilliseconds = 0;
        }

        public static void ReloadStats()
        {
            ReloadTimer();
            
            ChartPoints = new List<SpeedPoint>();
            AveragePoints = new List<SpeedPoint>();

            TypingErrors = 0;
            TypingSpeed = 0;
        }
    }
}