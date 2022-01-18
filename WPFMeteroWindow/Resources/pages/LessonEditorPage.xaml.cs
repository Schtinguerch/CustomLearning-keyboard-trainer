using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFMeteroWindow.Properties;
using Path = System.IO.Path;
using Microsoft.Win32;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using System.Windows.Media;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для LessonEditorPage.xaml
    /// </summary>
    public partial class LessonEditorPage : Page
    {
        private LessonEditor _editor;

        public LessonEditorPage()
        {
            InitializeComponent();
            LessonNameTextBox.Focus();

            Intermediary.LessonPage = this;

            if (Settings.Default.IsOpenUnderCourse)
            {
                var fileName = Settings.Default.LessonInCourseFileName;
                if (fileName == "empty")
                    NewLesson();
                else
                {
                    _editor = new LessonEditor(fileName);
                    EditorTitleTextBox.Text = $"{Path.GetFileName(fileName)} - {Localization.uLessonEditor}";
                    DisplayDataFromEditor();
                }
            }
            else
            {
                EditorTitleTextBox.Text = Path.GetFileName(Settings.Default.LoadedLessonFile) + $" - {Localization.uLessonEditor}";
                _editor = new LessonEditor(Settings.Default.LoadedLessonFile);
                DisplayDataFromEditor();
            }

            Intermediary.RichPresentManager.Update("Lesson editor", "Editing lesson...", "");
        }

        private void DisplayDataFromEditor()
        {
            LessonNameTextBox.Text = _editor.LessonName;
            LessonCpmTextBox.Text = _editor.NecessaryCPM.ToString();
            LessonMaxMistakesTextBox.Text = _editor.MaxAcceptableMistakes.ToString();
            LessonDataTextBox.Text = _editor.LessonText;
        }

        private void SaveLesson()
        {
            _editor.LessonName = LessonNameTextBox.Text;

            int necessaryCpm = 0;
            var isValidCpm = int.TryParse(LessonCpmTextBox.Text, out necessaryCpm);

            if (!isValidCpm)
                LessonCpmTextBox.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.KeyboardErrorHighlightColor);

            int maxMistakes = int.MaxValue;
            var isValidMaxMistakes = int.TryParse(LessonMaxMistakesTextBox.Text, out maxMistakes);

            if (!isValidMaxMistakes)
                LessonMaxMistakesTextBox.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.KeyboardErrorHighlightColor);

            if (!isValidCpm || !isValidMaxMistakes)
            {
                Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uInvalidDataInput}");
                return;
            }

            LessonCpmTextBox.BorderBrush = LessonMaxMistakesTextBox.BorderBrush =
                (SolidColorBrush)new BrushConverter().ConvertFromString("#121212");

            _editor.NecessaryCPM = necessaryCpm;
            _editor.MaxAcceptableMistakes = maxMistakes;
            _editor.LessonText = LessonDataTextBox.Text;
            
            _editor.WriteDataOnFile();
        }

        private void OpenLesson()
        {
            var openDialog = new OpenFileDialog()
            {
                DefaultExt = ".lml",
                Filter = "LML-Files|*.lml",
                Multiselect = false
            };

            if (openDialog.ShowDialog() == true)
            {
                EditorTitleTextBox.Text = Path.GetFileName(openDialog.FileName) + $" - {Localization.uLessonEditor}";
                _editor = new LessonEditor(openDialog.FileName);
                DisplayDataFromEditor();
            }
        }

        private void NewLesson()
        {
            _editor = new LessonEditor();

            EditorTitleTextBox.Text = $"New lesson - {Localization.uLessonEditor}";
            LessonNameTextBox.Text = LessonCpmTextBox.Text = LessonMaxMistakesTextBox.Text = LessonDataTextBox.Text = "";
        }

        private void SaveLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            SaveLesson();
        
        private void OpenLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            OpenLesson();

        private void LessonEditorPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && !Settings.Default.IsOpenUnderCourse)
                PageManager.HidePages();

            else
            {
                if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.O))
                    OpenLesson();

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.N))
                    NewLesson();

                else if (AppManager.IsComboKeyDown(e, Key.LeftAlt, Key.S))
                    SaveLesson();
            }
        }

        private void ClosePage()
        {
            if (Settings.Default.IsOpenUnderCourse)
                Intermediary.CoursePage.CloseLessonEditorPage();
            else
                PageManager.HidePages();
        }

        private void CancelLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            ClosePage();

        private void NewLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            NewLesson();
    }
}
