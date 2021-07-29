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

namespace WPFMeteroWindow
{
    public struct SpeedPoint
    {
        public float CPM;
        public int PartPoint;

        public SpeedPoint(int point, float cpm)
        {
            CPM = cpm;
            PartPoint = point;
        }
    }
    
    public partial class MainWindow : MetroWindow
    {
        private List<Key> _keysList = new List<Key>();

        private Storyboard _showMessageStoryboard;

        private bool _breakTextProcessing = false;

        private bool _isFirstMistake = true;

        private bool _isTyping;

        public bool IsTyping
        {
            get => _isTyping;
            set
            {
                _isTyping = value;
                Intermediary.RichPresentManager.Update(Settings.Default.ItTypingTest? "Typing test" : Settings.Default.LessonName, !value ? "Chilling..." : "Typing...", "");
            }
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

        public MainWindow()
        {
            InitializeComponent();
            
            KeyboardManager.Board = keyboardGrid;
            KeyboardManager.KeyboardPresenter = new KeyboardPresenter();
            KeyboardManager.HandPresenter = new HandPresenter(LeftHandFrame, RightHandFrame);
            
            PageManager.PageGrid = aoeiGrid;
            PageManager.PageFrame = SettingFrame;

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
        }

        public void ShowMessage(string message)
        {
            MessageTextBlock.Text = message;
            _showMessageStoryboard.Begin();
        }

        private void StartTypingDemo()
        {
            var typingTimer = new Timer();
            typingTimer.Interval = 80;
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

        private void BufferTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTyping)
                IsTyping = true;

            if (!_breakTextProcessing)
            {
                if (LessonManager.DoneRoad.Length == 0)
                    StatisticsManager.TypingTimer.Start();
                
                if (LessonManager.LeftRoad.Length >= 1)
                {
                    if (bufferTextBox.Text.Length > 0)
                    {
                        var lastCharacter = bufferTextBox.Text[bufferTextBox.Text.Length - 1];
                        if (lastCharacter == LessonManager.LeftRoad[0])
                        {
                            _isFirstMistake = true;
                            LessonManager.ErrorInput = "";

                            LessonManager.DoneRoad += lastCharacter;
                            LessonManager.LeftRoad = LessonManager.LeftRoad.Substring(1, LessonManager.LeftRoad.Length - 1);

                            _breakTextProcessing = true;
                            bufferTextBox.Text = "";

                            if (LessonManager.LeftRoad.Length >= 1)
                                KeyboardManager.ShowTypingHint(LessonManager.LeftRoad[0]);
                            else
                                LessonManager.EndLesson();
                        }
                        else
                        {
                            KeyboardManager.ShowTypingError(lastCharacter);
                            LessonManager.ErrorInput += lastCharacter;

                            if (_isFirstMistake)
                            {
                                StatisticsManager.TypingErrors++;
                                _isFirstMistake = false;
                            }
                        }
                    }
                }
                else
                    LessonManager.EndLesson();
            }
            else
            {
                _breakTextProcessing = false;
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            var selectedPage = TabPage.EmptyPage;

            if ((e.Key != Key.LeftCtrl) && (e.Key != Key.RightCtrl) && (PageManager.PageFrame.Source == null))
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

        private void OpenNewGameMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            PageManager.OpenPage(TabPage.ClickingGame);

        private void TextInputFrame_MouseDown(object sender, MouseButtonEventArgs e) =>
            bufferTextBox.Focus();

        private void TextInputFrame_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) =>
            bufferTextBox.Focus();
    }
}
