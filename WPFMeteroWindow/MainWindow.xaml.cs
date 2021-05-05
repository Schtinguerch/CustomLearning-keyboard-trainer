using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
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

            IsTyping = false;
        }

        public void StartPreviouslesson()
        {
            CourseManager.CurrentLessonIndex--;
            IsTyping = false;
        }


        public void StartNextLesson()
        {
            CourseManager.CurrentLessonIndex++;
            IsTyping = false;
        }
            

        public MainWindow()
        {
            InitializeComponent();
            
            KeyboardManager.Board = keyboardGrid;
            KeyboardManager.KeyboardPresenter = new KeyboardPresenter();
            KeyboardManager.HandPresenter = new HandPresenter(LeftHandFrame, RightHandFrame);
            
            PageManager.PageGrid = aoeiGrid;
            PageManager.PageFrame = SettingFrame;

            Intermediary.RichPresentManager = new DiscordManager();
            Intermediary.RichPresentManager.Initialize();

            AppManager.InitializeApplication();
            IsTyping = false;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            bufferTextBox.Focus();
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
                            errorInputTextBlock.Text = "";

                            LessonManager.DoneRoad += lastCharacter;
                            LessonManager.LeftRoad = 
                                LessonManager.LeftRoad.Substring(1, LessonManager.LeftRoad.Length - 1);

                            inputTextBox.Text = LessonManager.DoneRoad;
                            inputTextBlock.Text = LessonManager.LeftRoad;

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
                            errorInputTextBlock.Text += lastCharacter;

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

        private void AddTypingKeyToShowList(KeyEventArgs e)
        {
            var hasDuplicate = false;
            foreach (var key in _keysList)
                if (key == e.Key)
                {
                    hasDuplicate = true;
                    break;
                }

            if (!hasDuplicate)
                _keysList.Add(e.Key);

            ClickedKeysTextBlock.Text = _keysList.KeyListToString();
        }

        private void RemoveTypingKeyFromShowList(KeyEventArgs e)
        {
            _keysList.Remove(e.Key);
            ClickedKeysTextBlock.Text = _keysList.KeyListToString();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            AddTypingKeyToShowList(e);

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
            }

            if (selectedPage != TabPage.EmptyPage)
                PageManager.OpenPage(selectedPage);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            RemoveTypingKeyFromShowList(e);
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
    }
}
