using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPFMeteroWindow.Properties;
using WPFMeteroWindow.Controls;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System.Windows.Media;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для LessonIsEndPage.xaml
    /// </summary>
    public partial class LessonIsEndPage : Page
    {
        public LessonIsEndPage()
        {
            InitializeComponent();
            CurrLessonButton.Focus();
            MistakenCharsTextList.Orientation = Orientation.Horizontal;

            Intermediary.RichPresentManager.Update(Settings.Default.ItTypingTest ? "Typing test" : Settings.Default.LessonName, $"Ending: watching results, {StatisticsManager.TypingSpeedCpm:N} {Localization.uCPM}", "");

            foreach (var run in StatisticsManager.LessonRoadRuns)
                PassedLessonTextBlock.Inlines.Add(run);

            var lessonName = (Settings.Default.LessonName == "...") ? "" : $"\"{Settings.Default.LessonName}\"";
            EndedLessonHeaderTextBlock.Text = $"{Localization.uTheLesson} {lessonName} {Localization.uIsFinished}!!!";

            if (Settings.Default.ItTypingTest)
                EndedLessonHeaderTextBlock.Text = $"{Localization.uTypingTest} {Localization.uIsFinished}!!!";

            var typingSpeed = StatisticsManager.TypingSpeedCpm;
            var typingMistakePercentage = StatisticsManager.TypingMistakes / (float)LessonManager.DoneRoad.Length * 100d;

            WPMtextBlock.Text = $"{typingSpeed:N} {Localization.uCPM}";
            WPStextBlock.Text = $"{typingSpeed / 60f:N} {Localization.uCPS}";

            var typingSeconds = StatisticsManager.TypingMilliseconds / 1000d;

            ErrorsTextBlock.Text = $"{StatisticsManager.TypingMistakes} {Localization.uMistakes}: {typingMistakePercentage:N}%";
            TypingTimeTextBlock.Text = $"{Localization.uTime}: {typingSeconds:N}";
            CharactersCountTextBlock.Text = $"{Localization.uCharactersCount}: {LessonManager.DoneRoad.Length}";

            if (!StatisticsManager.IsDemonstrationMode)
            {
                var statistics = new LessonStatistics()
                {
                    AverageSpeed = StatisticsManager.TypingSpeedCpm,
                    MistakePercentage = typingMistakePercentage,
                    MistakenWords = StatisticsManager.MistakeWords,
                };

                StatisticsManager.GlobalTypingSpeeds.Add(statistics);

                if (Settings.Default.ItTypingTest)
                {
                    if (TestManager.Data.Results == null)
                        TestManager.Data.Results = new List<LessonStatistics>();

                    TestManager.Data.Results.Add(statistics);
                }

                else if (Settings.Default.IsCourseOpened)
                {
                    StatisticsManager.CourseStatistics.Add(statistics);
                }
            }

            if (!Settings.Default.IsCourseOpened && !Settings.Default.ItTypingTest)
                CourseStatsGrid.Height = 0;

            if (StatisticsManager.MistakeWords.Count == 0)
                MistakenDataGrid.Height = 0;

            var valuePlots = new List<List<double>>()
            {
                StatisticsManager.AverageSpeeds,
                StatisticsManager.WordSpeeds,
            };

            var timeList = new List<string>();
            int timeCount = 3;

            for (int i = 0; i < timeCount; i++)
            {
                var itemCount = StatisticsManager.TimePoints.Count;
                int offset = i == 2 ? 1 : 0;

                timeList.Add(StatisticsManager.TimePoints[itemCount * i / (timeCount - 1) - offset]);
            }

            CanvasGrid.Children.Add(
                new StatsVisualizer(
                    valuePlots, 
                    StatisticsManager.TimePoints, 
                    timeList, 
                    StatisticsManager.MistakePoints, 
                    StatisticsManager.MistakeCharacters, 
                    typingSpeed, 5));

            GlobalStatsGrid.Children.Add(
                new StatsVisualizer(
                    new List<List<double>>() 
                    { 
                        StatisticsManager.GetCpmFromStats(StatisticsManager.GlobalTypingSpeeds), 
                        StatisticsManager.GetPercentageFromStats(StatisticsManager.GlobalTypingSpeeds), 
                    }, null, null, null, null, -1, 3));

            var mainStatistcsData = StatisticsManager.GetCpmFromStats(StatisticsManager.CourseStatistics);
            var mistakeStatisticsData = StatisticsManager.GetPercentageFromStats(StatisticsManager.CourseStatistics);

            if (Settings.Default.ItTypingTest)
            {
                mainStatistcsData = StatisticsManager.GetCpmFromStats(TestManager.Data.Results);
                mistakeStatisticsData = StatisticsManager.GetPercentageFromStats(TestManager.Data.Results);
            }

            CoursePassingStatsGrid.Children.Add(
                new StatsVisualizer(
                    new List<List<double>>()
                    {
                        mainStatistcsData,
                        mistakeStatisticsData,
                    }, null, null, null, null, -1, 3));

            var reqCPM = Settings.Default.NecessaryCPM;
            var reqMistakes = Settings.Default.MaxAcceptableMistakes;
            var mistakes = StatisticsManager.TypingMistakes;

            var raidLessonStatus = false;
            var statusText = Localization.uCPM + ": ";

             if ((reqCPM != -1) && (reqMistakes != -1) && 
                !Settings.Default.ItTypingTest && Settings.Default.RequireWPM)
             {
                statusText += (typingSpeed > reqCPM) ? $"{typingSpeed} > {reqCPM}; " :
                    (typingSpeed == reqCPM) ? $"{typingSpeed} = {reqCPM}!!!; " :
                    $"{typingSpeed} < {reqCPM}!!!; ";

                statusText += Localization.uMistakes + ": ";
                statusText += (mistakes < reqMistakes) ? $"{mistakes} < {reqMistakes} -> " :
                    (mistakes == reqMistakes) ? $"{mistakes} = {reqMistakes}!!! -> " : 
                    $"{mistakes} > {reqMistakes}!!! -> " ;
                
                if ((typingSpeed >= reqCPM) && (mistakes <= reqMistakes))
                {
                    statusText += Localization.uSuccess;
                    raidLessonStatus = true;
                }
                else
                    statusText += Localization.uFail;

                LessonStatusTextBlock.Text = statusText;
            }
            else
            {
                raidLessonStatus = true;
                LessonStatusTextBlock.Text = "";
            }
            
            try
            {
                PrevLessonButton.Visibility = 
                    (Settings.Default.CourseLessonNumber == 0) 
                    || !Settings.Default.IsCourseOpened ? 
                        Visibility.Collapsed : Visibility.Visible;
                
                NextLessonButton.Visibility = 
                    (Settings.Default.CourseLessonNumber == CourseManager.LessonsCount - 1) 
                    || !Settings.Default.IsCourseOpened 
                    || !raidLessonStatus ? 
                        Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                PrevLessonButton.Visibility = Visibility.Collapsed;
                NextLessonButton.Visibility = Visibility.Collapsed;
            }

             if (Settings.Default.IsCourseOpened && !StatisticsManager.IsDemonstrationMode)
             {
                if (StatisticsManager.CourseMarks.FullyPassedLessons == null)
                    StatisticsManager.CourseMarks.FullyPassedLessons = new List<int>();

                if (StatisticsManager.CourseMarks.PartucularlyPassedLessons == null)
                    StatisticsManager.CourseMarks.PartucularlyPassedLessons = new List<int>();

                if (raidLessonStatus && !StatisticsManager.IsDemonstrationMode)
                {
                    if (!StatisticsManager.CourseMarks.FullyPassedLessons.Contains(CourseManager.CurrentLessonIndex))
                    {
                        StatisticsManager.CourseMarks.FullyPassedLessons.Add(CourseManager.CurrentLessonIndex);
                    }

                    StatisticsManager.CourseMarks.PartucularlyPassedLessons.Remove(CourseManager.CurrentLessonIndex);
                    Intermediary.App.PassedIndicator.Fill = XamlManager.FindResource<SolidColorBrush>("PassedIndicatorBrush");
                }
                else
                {
                    if (!StatisticsManager.CourseMarks.PartucularlyPassedLessons.Contains(CourseManager.CurrentLessonIndex) &&
                        !StatisticsManager.CourseMarks.FullyPassedLessons.Contains(CourseManager.CurrentLessonIndex))
                    {
                        StatisticsManager.CourseMarks.PartucularlyPassedLessons.Add(CourseManager.CurrentLessonIndex);
                    }

                    Intermediary.App.PassedIndicator.Fill = XamlManager.FindResource<SolidColorBrush>("FailedIndicatorBrush");
                }
            }

            MistakenWordsTextList.Height = Math.Min(MistakenCharsTextList.ItemsHeight, 300d);
            StatisticsManager.IsDemonstrationMode = false;

            MistakenWordsTextList.Items = StatisticsManager.MistakeWords;
            MistakenCharsTextList.Items = StatisticsManager.MistakeCharacters;
        }

        private void PrevLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageManager.HidePages();
            CourseManager.CurrentLessonIndex--;
            Intermediary.App.IsTyping = false;
        }

        private void CurrLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageManager.HidePages();
            
            if (Settings.Default.ItTypingTest)
                TestManager.RestartTest();
            else 
                CourseManager.CurrentLessonIndex = CourseManager.CurrentLessonIndex;

            Intermediary.App.IsTyping = false;
        }

        private void NextLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageManager.HidePages();
            CourseManager.CurrentLessonIndex++;
            Intermediary.App.IsTyping = false;
        }

        private void LessonIsEndPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (Settings.Default.ItTypingTest)
                    TestManager.RestartTest();
                else 
                    CourseManager.CurrentLessonIndex = CourseManager.CurrentLessonIndex;

                PageManager.HidePages();
                Intermediary.App.IsTyping = false;
            }
        }

        private void LessonIsEndPage_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
