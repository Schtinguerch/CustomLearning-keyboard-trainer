using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;
using Key = System.Windows.Input.Key;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using Visibility = System.Windows.Visibility;
using System.IO;
using System.Linq;
using Thread = System.Threading.Thread;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFMeteroWindow
{
    public partial class MainWindow : MetroWindow
    {
        private Storyboard 
            _showMessageStoryboard, 
            _shakeImageStoryboard, 
            _hideImageStoryboard, 
            _showBackImageStoryboard,
            _blurUpImageBackgroundStoryboard,
            _blurBackImageBackgroundStoryboard;

        private bool _breakTextProcessing = false;

        private bool _isFirstMistake = true;

        private bool _isTyping;

        private string _headerText;

        public bool IsTyping
        {
            get => _isTyping;
            set
            {
                _isTyping = value;
                Intermediary.RichPresentManager.Update(Settings.Default.ItTypingTest? "Typing test" : Settings.Default.LessonName, !value ? "Chilling..." : "Typing...", "");
            }
        }

        public void ShakeImage(bool hasToDo)
        {
            if (!hasToDo || !Settings.Default.IsBackgroundImage)
                return;

            _shakeImageStoryboard.Begin();
        }

        public void HideImage()
        {
            if (!Settings.Default.HideImageWhenLessonStart)
                return;

            _hideImageStoryboard.Begin();
        }

        public void ShowImage()
        {
            if (!Settings.Default.HideImageWhenLessonStart 
                || BackgroundImage.Opacity == 1d)
                return;

            _showBackImageStoryboard.Begin();
        }

        public void BlurImage()
        {
            if (!Settings.Default.BlurUpImageWhenLessonStart || 
                Settings.Default.BackgroundBlurRadius != "0")
                return;

            _blurUpImageBackgroundStoryboard.Begin();
        }

        public void UnblurImage()
        {
            if (!Settings.Default.BlurUpImageWhenLessonStart || 
                Settings.Default.BackgroundBlurRadius != "0" || 
                ImageBlurEffect.Radius == 0d)
                return;

            _blurBackImageBackgroundStoryboard.Begin();
        }

        public void RestartLesson()
        {
            if (Settings.Default.ItTypingTest)
                TestManager.RestartTest();
            else 
                CourseManager.CurrentLessonIndex = CourseManager.CurrentLessonIndex;

            bufferTextBox.Focus();
            IsTyping = false;
        }

        public void StartPreviouslesson()
        {
            CourseManager.CurrentLessonIndex--;

            bufferTextBox.Focus();
            IsTyping = false;
        }

        public void StartNextLesson()
        {
            CourseManager.CurrentLessonIndex++;

            bufferTextBox.Focus();
            IsTyping = false;
        }
            
        public void FocusOnTextInputControl() =>
            bufferTextBox.Focus();


        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();

            KeyboardManager.Board = keyboardGrid;
            KeyboardManager.KeyboardPresenter = new KeyboardPresenter();
            KeyboardManager.HandPresenter = new HandPresenter(LeftHandFrame, RightHandFrame);
            
            PageManager.PageGrid = aoeiGrid;
            PageManager.PageFrame = SettingFrame;

            PageManager.OpenPageStoryboard = FindResource("ShowPageFrameStoryboard") as Storyboard;
            PageManager.ClosePageStoryboard = FindResource("HidePageFrameStoryboard") as Storyboard;

            Intermediary.App = this;
            Intermediary.RichPresentManager = new DiscordManager();
            Intermediary.RichPresentManager.Initialize();

            LessonManager.TextInputPresenter = new LessonTextInputPresenter
            {
                TextInputFrame = TextInputFrame,
                TextInputControlName = Settings.Default.TextInputType,
            };
            
            IsTyping = false;
            Settings.Default.IsFirstTextInputOpen = true;

            _showMessageStoryboard = FindResource("ShowMessageStoryboard") as Storyboard;
            _shakeImageStoryboard = FindResource("ShakeImageStoryboard") as Storyboard;

            _hideImageStoryboard = FindResource("HideImageStoryboard") as Storyboard;
            _showBackImageStoryboard = FindResource("ShowBackImageStoryboard") as Storyboard;

            _blurUpImageBackgroundStoryboard = FindResource("BlurUpImageBackground") as Storyboard;
            _blurBackImageBackgroundStoryboard = FindResource("BlurBackImageBackground") as Storyboard;

            //The special window to test new app features
            //new TestWindow().Show();

            if (!File.Exists(Settings.Default.FirstLaunchFileIndicatorPath))
            {
                PageManager.OpenPage(TabPage.WelcomePage);
                File.WriteAllText(
                    Settings.Default.FirstLaunchFileIndicatorPath, 
                    "Yep dude!!!\n" +
                    "You can write me in Discord: Schtinguerch#7516\n" +
                    "Just send the screenshot with the message if you want...\n\n" +
                    "А если по-русски умеешь изъявляться, то вообще замечательно!\n" +
                    "Буду очень рад поболтать"); 
            }
        }

        public void ShowMessage(string message)
        {
            MessageTextBlock.Text = message;
            _showMessageStoryboard.Begin();
        }

        private void StartTypingDemo()
        {
            var typingTimer = new Timer();
            StatisticsManager.IsDemonstrationMode = true;

            typingTimer.Interval = 60;
            typingTimer.Start();

            typingTimer.Tick += (s, e) =>
            {
                try
                {
                    bufferTextBox.Text += LessonManager.LeftRoad[0];
                }

                catch
                {
                    typingTimer.Stop();
                }
            };
        }

        private bool _wasMistake = false;
        private Run _lastRun;

        private void BufferTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTyping)
                IsTyping = true;

            if (!_breakTextProcessing)
            {
                if (LessonManager.DoneRoad.Length == 0)
                {
                    StatisticsManager.StartTimer();
                    StatisticsManager.LessonRoadRuns.Add(_lastRun = new Run());
                    HideImage();
                    BlurImage();
                }

                if (LessonManager.LeftRoad.Length >= 1)
                {
                    if (bufferTextBox.Text.Length > 0)
                    {
                        var lastCharacter = bufferTextBox.Text[bufferTextBox.Text.Length - 1];
                        TypingProgressIndicator.Width = ActualWidth * StatisticsManager.PassPercentage / 100d;

                        var charactersEquals = lastCharacter == LessonManager.LeftRoad[0];

                        if (charactersEquals)
                        {
                            ShakeImage(Settings.Default.ShakeBackgroundInTyping);
                            SoundManager.PlayType();
                            LessonManager.ErrorInput = "";

                            LessonManager.DoneRoad += lastCharacter;
                            LessonManager.LeftRoad = LessonManager.LeftRoad.Substring(1, LessonManager.LeftRoad.Length - 1);

                            _isFirstMistake = true;
                            StatisticsManager.AddWordStatistics(lastCharacter);

                            _breakTextProcessing = true;
                            bufferTextBox.Text = "";

                            if (LessonManager.LeftRoad.Length >= 1)
                                KeyboardManager.ShowTypingHint(LessonManager.LeftRoad[0]);
                            else
                            {
                                ShowImage();
                                UnblurImage();
                                LessonManager.EndLesson();
                            }

                            if (_wasMistake)
                            {
                                _wasMistake = false;
                                StatisticsManager.LessonRoadRuns.Add(_lastRun = new Run());
                            }
                        }
                        else
                        {
                            KeyboardManager.ShowTypingError(lastCharacter);
                            LessonManager.ErrorInput += lastCharacter;

                            if (_isFirstMistake)
                            {
                                StatisticsManager.TypingMistakes++;
                                StatisticsManager.AddMistakeStatistics($"{LessonManager.LeftRoad[0]}");

                                SoundManager.PlayTypingMistake();

                                _isFirstMistake = false;
                                _wasMistake = true;

                                StatisticsManager.LessonRoadRuns.Add(
                                    _lastRun = new Run()
                                    {
                                        TextDecorations = TextDecorations.Strikethrough,
                                        Foreground = new BrushConverter().ConvertFromString(Settings.Default.KeyboardErrorHighlightColor) as SolidColorBrush
                                    });
                            }
                        }

                        _lastRun.Text += lastCharacter;
                    }
                }
                else
                {
                    ShowImage();
                    UnblurImage();
                    LessonManager.EndLesson();
                }
                    
            }
            else
            {
                _breakTextProcessing = false;
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            var selectedPage = TabPage.EmptyPage;

            if ((e.Key != Key.LeftCtrl) && (e.Key != Key.RightCtrl) && (e.Key != Key.Apps) && (PageManager.PageFrame.Source == null))
            {
                if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.F))
                {
                    e.Handled = true;

                    if (AppManager.IsComboKeyDown(e, Key.LeftShift) || AppManager.IsComboKeyDown(e, Key.RightShift))
                        Settings.Default.FontContext = "k:";
                    else
                        Settings.Default.FontContext = "l:";

                    selectedPage = TabPage.FontSetterShell;
                }
            
                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.C))
                {
                    e.Handled = true;
                    selectedPage = TabPage.CommandLine;
                }
            
                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.L))
                {
                    e.Handled = true;
                    selectedPage = TabPage.LayoutLoaderShell;
                }
            
                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.N))
                {
                    e.Handled = true;

                    if (AppManager.IsComboKeyDown(e, Key.LeftShift) || AppManager.IsComboKeyDown(e, Key.RightShift))
                        selectedPage = TabPage.CourseLoaderShell;
                    else
                        selectedPage = TabPage.LessonLoaderShell;
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.E))
                {
                    e.Handled = true;
                    Settings.Default.IsOpenUnderCourse = false;

                    if (AppManager.IsComboKeyDown(e, Key.LeftShift) || AppManager.IsComboKeyDown(e, Key.RightShift))
                        selectedPage = TabPage.CourseEditor;
                    else
                        selectedPage = TabPage.LessonEditor;
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.K))
                {
                    e.Handled = true;
                    selectedPage = TabPage.KeyboardLayoutEditor;
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.R))
                {
                    e.Handled = true;
                    LessonManager.RandomizeText = !LessonManager.RandomizeText;
                    RestartLesson();

                    RandomizedTextBlock.Text = LessonManager.RandomizeText ? $"{Localization.uShuffledWords} •" : "";
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.M))
                {
                    e.Handled = true;
                    RestartLesson();
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.D))
                {
                    e.Handled = true;
                    StartTypingDemo();
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.W))
                {
                    e.Handled = true;
                    Settings.Default.CurrentLayout = Settings.Default.CurrentLayout == 1 ? 2 : 1;

                    if (Settings.Default.CurrentLayout == 1)
                        Opener.NewKeyboardLayout(Settings.Default.KeyboardLayoutFile);
                    else 
                        Opener.NewKeyboardLayout(Settings.Default.SecondKeyboardLayoutFile);

                    Settings.Default.Save();
                }

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.U))
                {
                    e.Handled = true;
                    System.Diagnostics.Process.Start("CustomLearningUpdater.exe");
                }
            }

            if (selectedPage != TabPage.EmptyPage)
                PageManager.OpenPage(selectedPage);
        }

        private void PrevLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            StartPreviouslesson();

        private void NextLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            StartNextLesson();

        private void ReLesson_OnClick(object sender, RoutedEventArgs e) =>
            RestartLesson();

        private void OpenNewLessonMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            Opener.NewLessonViaExplorer();

        private void OpenNewCourseMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            Opener.NewCourseViaExplorer();

        private void OpenPreviousLessonMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            StartPreviouslesson();

        private void OpenNextLessonMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            StartNextLesson();

        private void QuitAppMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void AppSettingsMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.UserSettings);

        private void OpenLessonEditorMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.LessonEditor);

        private void OpenCourseEditorMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.CourseEditor);

        private void OpenLayoutEditorMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.KeyboardLayoutEditor);

        private void OpenRecentCourseMenuItem_Click(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.RecentCources);

        private void OpenRecentLayoutsMenuItem_Click(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.RecentLayouts);

        private void OpenRecentConfigsMenuItem_Click(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.RecentConfigs);

        private void TextInputFrame_MouseDown(object sender, MouseButtonEventArgs e) =>
            bufferTextBox.Focus();

        private void TextInputFrame_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) =>
            bufferTextBox.Focus();

        private void MetroWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePoint = e.GetPosition(MainGrid);
            ParallaxEffectPresenter.MakeParallaxEffect(mousePoint);
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdditionalInfoPanel.Margin = (Width < 1000d && WindowState == WindowState.Normal) ? 
                new Thickness(0, 0, 60, 0) : new Thickness(0, 0, 0, 0);

            AppTitle.Visibility = (Width < 1120d && WindowState == WindowState.Normal) ? 
                Visibility.Hidden : Visibility.Visible;
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e) =>
            MetroWindow_SizeChanged(null, null);

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Normal)
                    DragMove();
                else
                {
                    WindowState = WindowState.Normal;
                    Top = 50;
                }
            }
        }

        private void TextInputFrame_DragEnter(object sender, System.Windows.DragEventArgs e) =>
            DrapAndDropMessageGrid.Visibility = Visibility.Visible;

        private void TextInputFrame_DragLeave(object sender, System.Windows.DragEventArgs e) =>
            DrapAndDropMessageGrid.Visibility = Visibility.Hidden;

        private void Rectangle_DragEnter(object sender, System.Windows.DragEventArgs e) =>
            LayoutDrapAndDropMessageGrid.Visibility = Visibility.Visible;

        private void Rectangle_DragLeave(object sender, System.Windows.DragEventArgs e) =>
            LayoutDrapAndDropMessageGrid.Visibility = Visibility.Hidden;

        private void TextInputFrame_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                DrapAndDropMessageGrid.Visibility = Visibility.Hidden;
                return;
            }

            var files = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

            if (files == null || files.Length == 0)
            {
                DrapAndDropMessageGrid.Visibility = Visibility.Hidden;
                return;
            }

            var insertionFile = files[0];

            if (File.Exists(insertionFile))
            {
                if (files.Length == 1)
                    Opener.NewLesson(insertionFile);
                else
                {
                    var otherLessons = files.ToList();
                    var editor = new CourseEditor("TemporaryCourse", CourseState.Empty)
                    {
                        CourseName = Localization.uUserLessons,
                        Lessons = otherLessons,
                        Author = new AuthorData()
                        {
                            Name = "CustomLearningApp",
                            References = new List<string>()
                        }
                    };

                    editor.WriteDataOnFile();
                    Opener.NewCourse("TemporaryCourse", 0);
                }
            }
            else
            {
                for (int i = 0; i < files.Length; i++)
                    Opener.NewCourse(files[i], 0, true);

                Opener.NewCourse(insertionFile, -1);
            }

            DrapAndDropMessageGrid.Visibility = Visibility.Hidden;
        }

        private void Rectangle_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                LayoutDrapAndDropMessageGrid.Visibility = Visibility.Hidden;
                return;
            }
                

            var files = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

            if (files == null || files.Length == 0)
            {
                LayoutDrapAndDropMessageGrid.Visibility = Visibility.Hidden;
                return;
            }

            Opener.NewKeyboardLayout(files[0]);
            LayoutDrapAndDropMessageGrid.Visibility = Visibility.Hidden;
        }

        private void Grid_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            _headerText = lessonHeaderTextBlock.Text;
            lessonHeaderTextBlock.Text = Localization.uLoadUserAppStyle;
        }

        private void Grid_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            lessonHeaderTextBlock.Text = _headerText;
        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                lessonHeaderTextBlock.Text = _headerText;
                return;
            }

            var files = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

            if (files == null || files.Length == 0)
            {
                lessonHeaderTextBlock.Text = _headerText;
                return;
            }

            UserConfigManager.ImportConfigFromFile(files[0]);
            lessonHeaderTextBlock.Text = _headerText;
        }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            ContentRendered -= MetroWindow_ContentRendered;;
            ParallaxEffectPresenter.MakeParallaxEffect(new Point(0, 0));
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) =>
            AppManager.SaveJsonDataFromManagers();

        private void PassTypingTestItem_Click(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.TypingTestParameters);

        private void BackToLessonsButton_Click(object sender, RoutedEventArgs e) =>
            CourseManager.CurrentLessonIndex = CourseManager.CurrentLessonIndex;

        private void MetroWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShakeImage(Settings.Default.ShakeBackgroundInClicking);
            MouseSplashShape.StartSplash();

            var mousePoint = e.GetPosition(MainGrid);
            MouseSplashShape.Margin = new Thickness(mousePoint.X - 10, mousePoint.Y - 10, 0, 0);
        }  
    }
}
