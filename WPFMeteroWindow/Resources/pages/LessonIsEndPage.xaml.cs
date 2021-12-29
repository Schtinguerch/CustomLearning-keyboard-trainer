using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPFMeteroWindow.Properties;
using WPFMeteroWindow.Controls;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

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

            Intermediary.RichPresentManager.Update(Settings.Default.ItTypingTest ? "Typing test" : Settings.Default.LessonName, "Ending: watching results...", "");

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

            var valuePlots = new List<List<double>>()
            {
                StatisticsManager.AverageSpeeds,
                StatisticsManager.WordSpeeds,
            };

            foreach (var word in StatisticsManager.MistakeWords)
                MistakenWordsListBox.Items.Add(word);

            foreach (var character in StatisticsManager.MistakeCharacters)
                MistakenCharsListBox.Items.Add(character);

            var timeList = new List<string>();
            int timeCount = 3;

            for (int i = 0; i < timeCount; i++)
            {
                var itemCount = StatisticsManager.TimePoints.Count;
                int offset = i == 2 ? 1 : 0;

                timeList.Add(StatisticsManager.TimePoints[itemCount * i / (timeCount - 1) - offset]);
            }

            var plotDrawer = new StatsVisualizer(valuePlots, StatisticsManager.TimePoints, timeList, StatisticsManager.MistakePoints, StatisticsManager.MistakeCharacters, typingSpeed, 5);
            CanvasGrid.Children.Add(plotDrawer);
            
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
                        Visibility.Hidden : Visibility.Visible;
                
                NextLessonButton.Visibility = 
                    (Settings.Default.CourseLessonNumber == CourseManager.LessonsCount - 1) 
                    || !Settings.Default.IsCourseOpened 
                    || !raidLessonStatus ? 
                        Visibility.Hidden : Visibility.Visible;
            }
            catch
            {
                PrevLessonButton.Visibility = Visibility.Hidden;
                NextLessonButton.Visibility = Visibility.Hidden;
            }
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
