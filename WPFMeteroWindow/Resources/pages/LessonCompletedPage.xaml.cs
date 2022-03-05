using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Resources.TypingSpeedRanks.RankGrids;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для LessonCompletedPage.xaml
    /// </summary>
    public partial class LessonCompletedPage : Page
    {
        private float _currentTypingSpeed;

        public LessonCompletedPage()
        {
            InitializeComponent();
            LoadStatistics();

            IsVisibleChanged += (s, e) => 
            {
                if (!IsVisible) return;

                var showEndStoryboard = FindResource("ShowEndStoryboard") as Storyboard;
                showEndStoryboard.Begin();
            };
        }

        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            Intermediary.App.RestartLesson();
            PageManager.HidePages();
        }

        private void LoadComboBoxes()
        {
            foreach (var image in Intermediary.RecentImages)
                WindowColorType.Items.Add(image.Split('\\').Last());

            WindowColorType.Text = 
                Settings.Default.IsEndLessonBackgroundImage ? 
                    Settings.Default.EndLessonBackgroundImagePath.Split('\\').Last() : 
                    Localization.uSettBrush;

            RequireWPMtextBox.Text = 
                Settings.Default.RequireWPM ? 
                    Localization.uYes : 
                    Localization.uNo;
        }

        private void LoadStatistics()
        {
            LoadComboBoxes();
            SwitchFocusAndSetStatus();
            SetRankings();
            UnloadStatistics();
            LoadTextPassion();
            LoadMistakenData();
            LoadHeaderStatistics();
            LoadPlots();

            CheckResults();
            StatisticsManager.IsDemonstrationMode = false;
        }

        private void SetRankings()
        {
            if (StatisticsManager.IsDemonstrationMode)
            {
                //Auto typing mode: do not deserve rankings
                return;
            } 

            var rankConditions = new Func<float, bool>[]
            {
                x => x >= 0d,    //Beginner
                x => x >= 100d,  //Student
                x => x >= 200d,  //Bronze
                x => x >= 300d,  //Silver
                x => x >= 400d,  //Gold
                x => x >= 500d,  //Platine
                x => x >= 600d,  //Diamond
                x => x >= 700d,  //Hacker
                x => x >= 800d,  //Master
                x => x >= 900d, //Grandmaster
                x => x >= 1000d, //Overwhelmed
                x => x >= 1100d, //The Typing God
                x => x < 0       //Impossible condition
            };

            var rankGrids = new Control[]
            {
                new Beginner(),
                new Student(),
                new Bronze(),
                new Silver(),
                new Gold(),
                new Platinum(),
                new Diamond(),
                new Hacker(),
                new Master(),
                new Grandmaster(),
                new Overwhelmed(),
                new TheTypingGod(),
            };

            int rankIndex;
            for 
            (
                rankIndex = 0;
                rankConditions[rankIndex](_currentTypingSpeed) 
                && rankIndex != rankConditions.Count() - 1;
                rankIndex++
            );
            rankIndex -= 1;

            RankGrid.Children.Clear();
            RankGrid.Children.Add(rankGrids[rankIndex]);

            rankGrids = null;
            rankConditions = null;
            GC.Collect();
        }

        private void SwitchFocusAndSetStatus()
        {
            _currentTypingSpeed = StatisticsManager.TypingSpeedCpm;
            MistakePercentageTextBlock.Focus();

            Intermediary.RichPresentManager.Update(
                Settings.Default.ItTypingTest ? 
                    "Typing test" : Settings.Default.LessonName, 
                    "Ending: watching results, " + 
                    $"{StatisticsManager.TypingSpeedCpm:N} {Localization.uCPM}", 
                    "");
        }

        private void LoadHeaderStatistics()
        {
            TitleTextBlock.Text = Settings.Default.ItTypingTest?
                TestManager.HeaderName() :
                Settings.Default.LessonName;

            double mistakePercentage = 
                StatisticsManager.TypingMistakes 
                / (double) LessonManager.DoneRoad.Length 
                * 100d;

            TypingSpeedTextBlock.Text = $"{StatisticsManager.TypingSpeedCpm:N} {Localization.uCPM}";
            MistakePercentageTextBlock.Text = $"{mistakePercentage:N}%";
            TypingTimeTextBlock.Text = StatisticsManager.TimePoints.Last();

            MistakeCountTextBlock.Text = $"{StatisticsManager.TypingMistakes} {Localization.uMistakes}";
            TextLengthTextBlock.Text = $"{LessonManager.DoneRoad.Length} {Localization.uCharacters}";
        }

        private void LoadTextPassion()
        {
            var textBlock = new TextBlock()
            {
                Style = XamlManager.FindResource<Style>("SummaryTextStyle"),
                FontSize = 20d,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(0, 12, 0, 12),
            };

            foreach (var run in StatisticsManager.LessonRoadRuns)
            {
                textBlock.Inlines.Add(run);
            }

            TypingPassionIsle.HeaderTextBlock.Text = Localization.uTypingMistakes;
            TypingPassionIsle.SetChild(textBlock);
        }

        private void LoadPlots()
        {
            LoadCurrentLessonPlot();
            LoadStreakPlot();
            LoadGlobalPlot();
        }

        private void LoadCurrentLessonPlot()
        {
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

            LessonTypingSpeedIsle.HeaderTextBlock.Text = Localization.uTypingSpeedPlot;
            LessonTypingSpeedIsle.SetChild(
                new StatsVisualizer(
                    valuePlots,
                    StatisticsManager.TimePoints,
                    timeList,
                    StatisticsManager.MistakePoints,
                    StatisticsManager.MistakeCharacters,
                    StatisticsManager.TypingSpeedCpm, 5)
                {
                    Height = 300d,
                    Margin = new Thickness(0, 12, 0, 12),
                });
        }

        private void LoadMistakenData()
        {
            if (
                StatisticsManager.MistakeCharacters.Count == 0 &&
                StatisticsManager.MistakeWords.Count == 0)
            {
                ItemsStackPanel.Children.Remove(MistakenDataIsle);
                return;
            }

            MistakenDataIsle.HeaderTextBlock.Text = Localization.uMistakenWordsAndChars;
            var grid = new Grid() { Height = 300d };

            var wordTextList = new TextList() { Height = 300d };
            wordTextList.Items = StatisticsManager.MistakeWords;

            var charTextList = new TextList() { Height = 300d };
            charTextList.Items = StatisticsManager.MistakeCharacters;

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(wordTextList);
            grid.Children.Add(charTextList);

            Grid.SetColumn(wordTextList, 0);
            Grid.SetColumn(charTextList, 1);

            MistakenDataIsle.SetChild(grid);
        }
    
        private void LoadStreakPlot()
        {
            if (!Settings.Default.IsCourseOpened && !Settings.Default.ItTypingTest)
            {
                ItemsStackPanel.Children.Remove(StreakStatisticsIsle);
                return;
            }

            List<double> mainStatistcsData, mistakeStatisticsData;
            if (Settings.Default.ItTypingTest)
            {
                mainStatistcsData = StatisticsManager.GetCpmFromStats(TestManager.Data.Results);
                mistakeStatisticsData = StatisticsManager.GetPercentageFromStats(TestManager.Data.Results);
                StreakStatisticsIsle.HeaderTextBlock.Text = Localization.uTestPassingStatistics;
            }
            else
            {
                mainStatistcsData = StatisticsManager.GetCpmFromStats(StatisticsManager.CourseStatistics);
                mistakeStatisticsData = StatisticsManager.GetPercentageFromStats(StatisticsManager.CourseStatistics);
                StreakStatisticsIsle.HeaderTextBlock.Text = Localization.uCoursePassingStats;
            }

            StreakStatisticsIsle.SetChild(
                new StatsVisualizer(
                    new List<List<double>>()
                    {
                        mainStatistcsData,
                        mistakeStatisticsData,
                    }, null, null, null, null, -1, 3)
                {
                    Height = 300d,
                    Margin = new Thickness(0, 12, 0, 12),
                });
        }

        private void LoadGlobalPlot()
        {
            GlobalStatisticsIsle.HeaderTextBlock.Text = Localization.uGlobalPassingStats;
            GlobalStatisticsIsle.SetChild(
                new StatsVisualizer(
                    new List<List<double>>()
                    {
                        StatisticsManager.GetCpmFromStats(StatisticsManager.GlobalTypingSpeeds),
                        StatisticsManager.GetPercentageFromStats(StatisticsManager.GlobalTypingSpeeds),
                    }, null, null, null, null, -1, 5)
                {
                    Height = 300d,
                    Margin = new Thickness(0, 12, 0, 12),
                });
        }
            
        private void UnloadStatistics()
        {
            if (StatisticsManager.IsDemonstrationMode) return;
            
            double mistakePercentage =
                StatisticsManager.TypingMistakes
                / (double)LessonManager.DoneRoad.Length
                * 100d;

            var statistics = new LessonStatistics()
            {
                AverageSpeed = StatisticsManager.TypingSpeedCpm,
                MistakePercentage = mistakePercentage,
                MistakenWords = StatisticsManager.MistakeWords,
            };

            StatisticsManager.GlobalTypingSpeeds.Add(statistics);

            if (Settings.Default.ItTypingTest)
            {
                if (TestManager.Data.Results == null)
                {
                    TestManager.Data.Results = new List<LessonStatistics>();
                }

                TestManager.Data.Results.Add(statistics);
            }

            else if (Settings.Default.IsCourseOpened)
            {
                StatisticsManager.CourseStatistics.Add(statistics);
            }
        }

        private void UnloadCourseStatistics(bool raidLessonStatus)
        {
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
        }

        private void CheckResults()
        {
            MistakesCheckTextBox.Text = 
                $"{StatisticsManager.TypingMistakes} / " + 
                $"{Settings.Default.MaxAcceptableMistakes} " + 
                $"{Localization.uMistakes}";

            TypingSpeedCheckTextBox.Text =
                $"{StatisticsManager.TypingSpeedCpm} / " +
                $"{Settings.Default.NecessaryCPM} " +
                $"{Localization.uCPM}";

            if (!StatisticsManager.IsDemonstrationMode)
            {
                if (Settings.Default.ItTypingTest)
                {
                    if (AreEqual(_currentTypingSpeed, TestManager.Data.Results.Max(x => x.AverageSpeed)))
                    {
                        BestResultGrid.Visibility = Visibility.Visible;
                    }
                }

                else if (Settings.Default.IsCourseOpened)
                {
                    if (AreEqual(_currentTypingSpeed, StatisticsManager.CourseStatistics.Max(x => x.AverageSpeed)))
                    {
                        BestResultGrid.Visibility = Visibility.Visible;
                    }
                }
            }

            if (!Settings.Default.RequireWPM || StatisticsManager.IsDemonstrationMode)
            {
                IndicatorsStackPanel.Children.Remove(TypingMistakesIndicatorGrid);
                IndicatorsStackPanel.Children.Remove(TypingSpeedIndicatorGrid);

                UnloadCourseStatistics(
                    StatisticsManager.TypingSpeedCpm >= Settings.Default.NecessaryCPM &&
                    StatisticsManager.TypingMistakes <= Settings.Default.MaxAcceptableMistakes);

                CheckCourseBorders();
                return;
            }

            var passed = true;
            if (StatisticsManager.TypingSpeedCpm < Settings.Default.NecessaryCPM)
            {
                passed = false;
                TypingSpeedIndicatorRectangle.Fill = XamlManager.FindResource<SolidColorBrush>("ErrorIsleBrush");
                TypingSpeedIndicatorTextBlock.Text = "\uEF2C";
                LoadNextLessonButton.Visibility = Visibility.Collapsed;
            }

            if (StatisticsManager.TypingMistakes > Settings.Default.MaxAcceptableMistakes)
            {
                passed = false;
                TypingMistakesIndicatorRectangle.Fill = XamlManager.FindResource<SolidColorBrush>("ErrorIsleBrush");
                TypingMistakesIndicatorTextBlock.Text = "\uEF2C";
                LoadNextLessonButton.Visibility = Visibility.Collapsed;
            }

            UnloadCourseStatistics(passed);
            if (!passed) return;

            VerifiedResultGrid.Visibility = Visibility.Visible;
            CheckCourseBorders();
        }

        private void CheckCourseBorders()
        {
            try
            {
                LoadPreviousLessonButton.Visibility =
                    (Settings.Default.CourseLessonNumber == 0)
                    || !Settings.Default.IsCourseOpened ?
                        Visibility.Collapsed : Visibility.Visible;

                LoadNextLessonButton.Visibility =
                    (Settings.Default.CourseLessonNumber == CourseManager.LessonsCount - 1)
                    || !Settings.Default.IsCourseOpened ?
                        Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                LoadPreviousLessonButton.Visibility = Visibility.Collapsed;
                LoadNextLessonButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadPreviousLessonButton_Click(object sender, RoutedEventArgs e)
        {
            Intermediary.App.StartPreviouslesson();
            PageManager.HidePages();
        }

        private void ReloadLessonButton_Click(object sender, RoutedEventArgs e)
        {
            Intermediary.App.RestartLesson();
            PageManager.HidePages();
        }

        private void LoadNextLessonButton_Click(object sender, RoutedEventArgs e)
        {
            Intermediary.App.StartNextLesson();
            PageManager.HidePages();
        }

        private bool AreEqual(double x, double y) =>
            Math.Abs(x - y) < Math.Pow(10, -6);

        private void GetScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderTargetBitmap = 
                new RenderTargetBitmap(
                    (int) Intermediary.App.ActualWidth, 
                    (int) Intermediary.App.ActualHeight, 
                    96, 96, 
                    PixelFormats.Pbgra32);

            renderTargetBitmap.Render(Intermediary.App);
            Clipboard.SetImage(renderTargetBitmap);
        }

        private void RequireWPMtextBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SetYesOrNo("WPM", (RequireWPMtextBox.SelectedItem as TextBlock).Text);

        private void SetYesOrNo(string target, string text)
        {
            var state = true;

            if (text.ToLower() == Localization.uNo.ToLower())
                state = false;

            switch (target)
            {
                case "WPM":
                    Settings.Default.RequireWPM = state;
                    break;
            }

            Settings.Default.Save();
        }

        private void WindowColorType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (WindowColorType.SelectedIndex)
            {
                case 0:
                    BackgroundImage.Source = null;
                    Settings.Default.IsEndLessonBackgroundImage = false;
                    break;

                case 1:
                    SetImage(Opener.ImageViaExplorer());
                    break;

                default:
                    int index = WindowColorType.SelectedIndex - 2;
                    SetImage(Intermediary.RecentImages[index]);
                    break;
            }
        }

        private void SetImage(string filename)
        {
            if (!File.Exists(filename))
            {
                Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uInvalidDataInput}");
                return;
            }

            BackgroundImage.Source = new BitmapImage(new Uri(filename));

            if (!Intermediary.RecentImages.Contains(filename))
                Intermediary.RecentImages.Add(filename);

            Settings.Default.IsEndLessonBackgroundImage = true;
            Settings.Default.EndLessonBackgroundImagePath = filename;
            Settings.Default.Save();
        }

        private bool _show = true;
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _show = !_show;

            ButtonBeginStoryboard.Storyboard = _show ?
                FindResource("HideSettingsPanelStoryborad") as Storyboard :
                FindResource("ShowSettingsPanelStoryborad") as Storyboard;
        }
    }
}
