using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using LmlLibrary;
using WPFMeteroWindow.Properties;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

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
        private string _doneRoad = "";

        private string _leftRoad = "";

        private (string[][] keys, Button[] buttons) _keyData;

        private bool _breakTextProcessing = false;

        private List<string> _lessons;

        public List<SpeedPoint> ChartPoints;

        public List<SpeedPoint> AveragePoints;

        public int LessonsCount { get; private set; } = 0;

        private int _typingErrors = 0;

        private Timer _typingTimer;

        private int _typingMilliseconds = 0;

        private int _stoppedDoneRoad;

        private bool _isFirstMistake = true;

        private int _currentLessonIndex
        {
            get => Settings.Default.CourseLessonNumber;
            set
            {
                try
                {
                    Settings.Default.CourseLessonNumber = value;
                    Settings.Default.Save();
                
                    ReloadTimer();
                    if (Settings.Default.IsCourseOpened)
                        LoadLesson(_lessons[value]);
                    else
                        LoadLesson(Settings.Default.LoadedLessonFile);
                    
                        

                    PrevLessonButton.Visibility = (value == 0) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                    NextLessonButton.Visibility = (value == _lessons.Count - 1) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                }
                catch
                {
                    
                }
            }
        }

        public void InitiateResourceDictionaries()
        {
            var appDictionary = Application.Current.Resources.MergedDictionaries;
            appDictionary[appDictionary.Count - 2] = new ResourceDictionary()
            {
                Source = new Uri(Settings.Default.ThemeResourceDictionary, UriKind.RelativeOrAbsolute)
            };
            appDictionary[appDictionary.Count - 3] = new ResourceDictionary()
            {
                Source = new Uri(Settings.Default.ColorSchemeResourceDictionary, UriKind.RelativeOrAbsolute)
            };
        }

        public void SetNewLetterFont()
        {
            inputTextBlock.FontFamily = new FontFamily(Settings.Default.LessonLettersFont);
            inputTextBox.FontFamily = new FontFamily(Settings.Default.LessonLettersFont);
        }

        public void ReloadKeyboard()
        {
            _keyData = keyboardGrid.LoadButtons(Settings.Default.KeyboardLayoutFile);
            ShowTheNecessaryButtons(_leftRoad[0]);
        }

        public void HideSettingGrid()
        {
            aoeiGrid.Visibility = Visibility.Hidden;
            SettingFrame.Source = null;
        }

        public void RestartLesson() =>
            _currentLessonIndex = _currentLessonIndex;

        public void StartPreviouslesson() =>
            _currentLessonIndex--;

        public void StartNextLesson() =>
            _currentLessonIndex++;

        public MainWindow()
        {
            InitializeComponent();
            Actions.FindMainWindow();
            InitiateResourceDictionaries();
            PrevLessonButton.Visibility = Actions.IsVisible(Settings.Default.IsCourseOpened);
            NextLessonButton.Visibility = Actions.IsVisible(Settings.Default.IsCourseOpened);

            SetNewLetterFont();
            int errorIndex = -1;

            try
            {
                errorIndex++;
                _keyData = keyboardGrid.LoadButtons(Settings.Default.KeyboardLayoutFile);

                errorIndex++;
                if (Settings.Default.IsCourseOpened)
                {
                    errorIndex++;
                    LoadCourse(Settings.Default.LoadedCourseFile);
                }
                else
                    LoadLesson(Settings.Default.LoadedLessonFile);
            }
            catch (Exception ex)
            {
                var isNecessaryFileExists = true;
                
                switch (errorIndex)
                {
                    case 0:
                        isNecessaryFileExists = File.Exists(Settings.Default.KeyboardLayoutFile);
                        Opener.NewKeyboardLayout("DefaultData\\SchtDvorak.lml");
                        break;
                    
                    case 1:
                        isNecessaryFileExists = File.Exists(Settings.Default.LoadedLessonFile);
                        Opener.NewLesson("DefaultData\\Monday.lml");
                        break;
                    
                    case 2:
                        isNecessaryFileExists = File.Exists(Settings.Default.LoadedCourseFile);
                        Opener.NewLesson("DefaultData\\Monday.lml");
                        break;
                }
                
                MessageBox.Show("Ошибка: не удаётся открыть файл, " + 
                                (isNecessaryFileExists? 
                                    "ошибка доступа" : "файл не существует!!!"));
            }
        }

        private void ShowTheNecessaryButtons(char character)
        {
            int keyIndex = -1, statusIndex = -1;

            try
            {
                for (int i = 0; i < 61; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((_keyData.keys[i][j] != "") && (_keyData.keys[i][j] != null))
                        {
                            _keyData.buttons[i].Background =
                                new BrushConverter().ConvertFromString(Settings.Default.KeyboardBackgroundColor)
                                    as SolidColorBrush;

                            var keyCode = _keyData.keys[i][j];
                            
                            if (keyCode[0] == character && keyCode.Length < 2 || (keyCode.Length == 2 && keyCode[1] == character))
                            {
                                keyIndex = i;
                                statusIndex = j;
                            }
                        }
                    }
                }

                if (keyIndex == 56)
                {
                    rightHandTextBlock.Text = $"{Localization.uThumb} {Localization.uSpace}";
                    leftHandTextBlock.Text = "";
                }
                else if (keyIndex.IsInRange(0, 6) || keyIndex.IsInRange(15, 19) || keyIndex.IsInRange(29, 33) ||
                         keyIndex.IsInRange(42, 46))
                {
                    rightHandTextBlock.Text = "";
                    leftHandTextBlock.Text = Localization.uLeft;
                    if (keyIndex.IsEqualOr(0, 1, 15, 29, 42))
                    {
                        leftHandTextBlock.Text += Localization.uPinky + _keyData.keys[keyIndex][0].ToUpper();
                    }
                    else if (keyIndex.IsEqualOr(2, 16, 30, 43))
                    {
                        leftHandTextBlock.Text += Localization.uRing + _keyData.keys[keyIndex][0].ToUpper();
                    }
                    else if (keyIndex.IsEqualOr(3, 17, 31, 44))
                    {
                        leftHandTextBlock.Text += Localization.uMiddle + _keyData.keys[keyIndex][0].ToUpper();
                    }
                    else
                    {
                        leftHandTextBlock.Text += Localization.uIndex + _keyData.keys[keyIndex][0].ToUpper();
                    }

                    if (statusIndex == 0)
                    {
                        rightHandTextBlock.Text = "";
                    }
                    else if (statusIndex == 1)
                    {
                        _keyData.buttons[52].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uPinky}Shift";
                    }
                    else if (statusIndex == 2)
                    {
                        _keyData.buttons[57].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uThumb}AltGr";
                    }
                    else if (statusIndex == 3)
                    {
                        _keyData.buttons[52].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        _keyData.buttons[57].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uPinky}Shift + {Localization.uRight}{Localization.uThumb}AltGr";
                    }
                }
                else
                {
                    leftHandTextBlock.Text = "";
                    rightHandTextBlock.Text = Localization.uRight;

                    if (keyIndex.IsEqualOr(13, 12, 11, 10, 27, 26, 25, 24, 40, 39, 38, 52, 51))
                    {
                        rightHandTextBlock.Text += Localization.uPinky + _keyData.keys[keyIndex][0].ToUpper();
                    }
                    else if (keyIndex.IsEqualOr(9, 23, 37, 50))
                    {
                        rightHandTextBlock.Text += Localization.uRing + _keyData.keys[keyIndex][0].ToUpper();
                    }
                    else if (keyIndex.IsEqualOr(8, 22, 36, 49))
                    {
                        rightHandTextBlock.Text += Localization.uMiddle + _keyData.keys[keyIndex][0].ToUpper();
                    }
                    else
                    {
                        rightHandTextBlock.Text += Localization.uIndex + _keyData.keys[keyIndex][0].ToUpper();
                    }


                    if (statusIndex == 0)
                    {
                        rightHandTextBlock.Text += "";
                    }
                    else if (statusIndex == 1)
                    {
                        _keyData.buttons[41].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        leftHandTextBlock.Text += $" + {Localization.uLeft}{Localization.uPinky}Shift";
                    }
                    else if (statusIndex == 2)
                    {
                        _keyData.buttons[57].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        leftHandTextBlock.Text += $" + {Localization.uRight}{Localization.uThumb}AltGr";
                    }
                    else if (statusIndex == 3)
                    {
                        _keyData.buttons[41].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        _keyData.buttons[57].Background =
                            new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                                as SolidColorBrush;
                        
                        leftHandTextBlock.Text += $" + {Localization.uLeft}{Localization.uPinky}Shift + {Localization.uRight}{Localization.uThumb}AltGr";
                    }
                }

                _keyData.buttons[keyIndex].Background = Background =
                    new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor) as SolidColorBrush;
            }
            catch
            {

            }
        }

        private void ShowError(char errorCharacter)
        {
            int keyIndex = -1, statusIndex = -1;

            try
            {
                for (int i = 0; i < 61; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((_keyData.keys[i][j] != "") && (_keyData.keys[i][j] != null))
                            if (_keyData.keys[i][j][0] == errorCharacter && _keyData.keys[i][j].Length < 2)
                            {
                                keyIndex = i;
                                statusIndex = j;
                            }
                    }
                }

                _keyData.buttons[keyIndex].Background = Brushes.Red;
            }
            catch
            {
                
            }
        }

        public void LoadLesson(string filename)
        {
            var reader = new Lml(filename, Lml.Open.FromFile);
            string lessonText, lessonName;
            int necessaryWPM;
            try
            {
                if (Settings.Default.IsCourseOpened)
                    lessonName = Settings.Default.CourseName + ": " + reader.GetString("Lesson>Name");
                else
                    lessonName = reader.GetString("Lesson>Name");
                
                lessonText = Regex.Replace(reader.GetString("Lesson>Text"), "\\s+", " ");
                necessaryWPM = reader.GetInt("Lesson>NecessaryWPM");
            }
            catch
            {
                lessonName = "...";
                lessonText = File.ReadAllText(filename).Replace("\n", " ");
            }

            _doneRoad = "";
            inputTextBox.Text = "";
            ChartPoints = new List<SpeedPoint>();
            AveragePoints = new List<SpeedPoint>();

            _leftRoad = lessonText;
            inputTextBlock.Text = lessonText;
            lessonHeaderTextBlock.Text = lessonName;
            
            Settings.Default.LessonName = lessonName;
            Settings.Default.Save();
            
            NextLessonButton.Visibility = Actions.IsVisible(Settings.Default.IsCourseOpened);
            PrevLessonButton.Visibility = Actions.IsVisible(Settings.Default.IsCourseOpened);

            ShowTheNecessaryButtons(_leftRoad[0]);
            _typingErrors = 0;
            ReloadTimer();
        }

        public void ReloadTimer()
        {
            if (_typingTimer != null)
                _typingTimer.Stop();
            
            _typingTimer = new Timer();
            _typingTimer.Interval = 94;
            _typingTimer.Tick += TickTuck;

            _typingMilliseconds = 0;
            TimerTextBlock.Text = "0:0";
        }

        private void TickTuck(object sender, EventArgs e)
        {
            _typingMilliseconds++;
            TimerTextBlock.Text = _typingMilliseconds / 10 + ":" + _typingMilliseconds % 10;

            if (_typingMilliseconds % 10 == 0)
            {
                ChartPoints.Add(new SpeedPoint(_typingMilliseconds / 10 - 1, _doneRoad.Length - _stoppedDoneRoad));
                AveragePoints.Add(new SpeedPoint(_typingMilliseconds / 10 -1, (float) _doneRoad.Length / _typingMilliseconds * 10));
                _stoppedDoneRoad = _doneRoad.Length;
            }
                
        }
        
        public void LoadCourse(string filename)
        {
            if (filename.Contains(".lml"))
            {
                filename = new DirectoryInfo(filename).Parent.Name;
            }
            
            var reader = new Lml(filename + "\\CourseLessons.lml", Lml.Open.FromFile);
            var lessonFiles =  Actions.GetFileList(reader.GetArray("Course>LessonList"));

            for (int i = 0; i < lessonFiles.Count; i++)
                lessonFiles[i] = new DirectoryInfo(filename).FullName + "\\" + lessonFiles[i];

            _lessons = lessonFiles;
            LessonsCount = _lessons.Count;

            LoadLesson(lessonFiles[Settings.Default.CourseLessonNumber]);
            Settings.Default.LoadedLessonFile = lessonFiles[Settings.Default.CourseLessonNumber];
            Settings.Default.LoadedCourseFile = filename;
            Settings.Default.CourseName = reader.GetString("Course>Name");
            Settings.Default.Save();

            NextLessonButton.Visibility = Visibility.Visible;
            PrevLessonButton.Visibility = Visibility.Visible;

            _currentLessonIndex = Settings.Default.CourseLessonNumber;
        }

        private void EndLesson()
        {
            Settings.Default.TypingTime = _typingMilliseconds;
            Settings.Default.TypingLength = _doneRoad.Length;
            Settings.Default.TypingErrors = _typingErrors;
            Settings.Default.Save();
            
            ChartPoints.Add(new SpeedPoint(_typingMilliseconds / 10 , _doneRoad.Length - _stoppedDoneRoad));
            AveragePoints.Add(new SpeedPoint(_typingMilliseconds / 10 , (float) _doneRoad.Length / _typingMilliseconds * 10));

            ReloadTimer();
            aoeiGrid.Visibility = Visibility.Visible;
            SettingFrame.Source = new Uri(@"Resources/pages/LessonIsEndPage.xaml", UriKind.Relative);
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            bufferTextBox.Focus();
        }

        private void BufferTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_breakTextProcessing)
            {
                if (_doneRoad.Length == 0)
                    _typingTimer.Start();
                
                if (_leftRoad.Length >= 1)
                {
                    if (bufferTextBox.Text.Length > 0)
                    {
                        var lastCharacter = bufferTextBox.Text[bufferTextBox.Text.Length - 1];
                        if (lastCharacter == _leftRoad[0])
                        {
                            _isFirstMistake = true;

                            _doneRoad += lastCharacter;
                            _leftRoad = _leftRoad.Substring(1, _leftRoad.Length - 1);

                            inputTextBox.Text = _doneRoad;
                            inputTextBlock.Text = _leftRoad;

                            _breakTextProcessing = true;
                            bufferTextBox.Text = "";

                            if (_leftRoad.Length >= 1)
                                ShowTheNecessaryButtons(_leftRoad[0]);
                            else
                                EndLesson();
                        }
                        else
                        {
                            ShowError(lastCharacter);

                            if (_isFirstMistake)
                            {
                                _typingErrors++;
                                _isFirstMistake = false;
                            }
                        }
                    }
                }
                else
                    EndLesson();
            }
            else
            {
                _breakTextProcessing = false;
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Actions.IsComboKeyDown(e, Key.LeftAlt, Key.F))
            {
                e.Handled = true;

                if (Actions.IsComboKeyDown(e, Key.LeftShift) || Actions.IsComboKeyDown(e, Key.RightShift))
                {
                    Settings.Default.FontContext = "k:";
                }
                else
                {
                    Settings.Default.FontContext = "l:";
                }

                Settings.Default.Save();

                aoeiGrid.Visibility = Visibility.Visible;
                SettingFrame.Source = new Uri(@"Resources/pages/FontSettingPage.xaml", UriKind.Relative);

            }
            else if (Actions.IsComboKeyDown(e, Key.LeftAlt, Key.C))
            {
                e.Handled = true;

                aoeiGrid.Visibility = Visibility.Visible;
                SettingFrame.Source = new Uri(@"Resources/pages/CommandLinePage.xaml", UriKind.Relative);
            }
            else if (Actions.IsComboKeyDown(e, Key.LeftAlt, Key.L))
            {
                e.Handled = true;

                aoeiGrid.Visibility = Visibility.Visible;
                SettingFrame.Source = new Uri(@"Resources/pages/LayoutChangerPage.xaml", UriKind.Relative);
            }
            else if (Actions.IsComboKeyDown(e, Key.LeftAlt, Key.N))
            {
                e.Handled = true;
                aoeiGrid.Visibility = Visibility.Visible;

                if (Actions.IsComboKeyDown(e, Key.LeftShift) || Actions.IsComboKeyDown(e, Key.RightShift))
                {
                    SettingFrame.Source = new Uri("Resources/pages/CourseLoaderPage.xaml", UriKind.Relative);
                }
                else
                {
                    SettingFrame.Source = new Uri(@"Resources/pages/LessonLoaderPage.xaml", UriKind.Relative);
                }
            }
            else if (e.Key == Key.Escape)
            {
                HideSettingGrid();
            }
        }

        private void PrevLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            _currentLessonIndex--;
        
        private void NextLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            _currentLessonIndex++;

        private void ReLesson_OnClick(object sender, RoutedEventArgs e) =>
            _currentLessonIndex = _currentLessonIndex;

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            aoeiGrid.Visibility = Visibility.Visible;
            SettingFrame.Source = new Uri("Resources/pages/ApplicationUserSettingsPage.xaml", UriKind.Relative);
        }
    }
}
