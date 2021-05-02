using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using Path = System.IO.Path;
using Microsoft.Win32;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для CourseEditorPage.xaml
    /// </summary>
    public partial class CourseEditorPage : Page
    {
        private CourseEditor _editor;

        public CourseEditorPage()
        {
            InitializeComponent();
            Settings.Default.LessonInCourseFileName = "empty";
            Settings.Default.IsOpenUnderCourse = true;
            Intermediary.CoursePage = this;
            LessonListTextBox.Focus();

            _editor = new CourseEditor(Settings.Default.LoadedCourseFile, CourseState.NotEmpty);
            DisplayDataFromEditor();
        }

        public void AddNewLesson(string path)
        {
            var addingPath = path;

            if (path.Contains(_editor.FolderPath))
                addingPath = path.Replace(_editor.FolderPath + '\\', "");

            LessonListTextBox.Text += addingPath + '\n';
        }

        private void DisplayDataFromEditor()
        {
            CourseNameTextBox.Text = _editor.CourseName;
            AuthorTextBox.Text = _editor.Author.Name;
            LessonListTextBox.Text = _editor.ListToString(_editor.Lessons, 0);
            ReferencesListTextBox.Text = _editor.ListToString(_editor.Author.References, 0);
            EditorTitleTextBox.Text = $"{_editor.CourseName} - {Localization.uCourseEditor}";
        }

        private void SendDataToEditor()
        {
            _editor.CourseName = CourseNameTextBox.Text;
            _editor.Lessons = _editor.StringToList(LessonListTextBox.Text);

            _editor.Author = new AuthorData()
            {
                Name = AuthorTextBox.Text,
                References = _editor.StringToList(ReferencesListTextBox.Text),
            };
        }

        private void NewCourse()
        {
            _editor = new CourseEditor();

            EditorTitleTextBox.Text = $"{Localization.uNewCourse} - {Localization.uCourseEditor}";
            CourseNameTextBox.Text = AuthorTextBox.Text = LessonListTextBox.Text = ReferencesListTextBox.Text = "";
        }

        private void OpenCourse()
        {
            var openDialog = new OpenFileDialog()
            {
                DefaultExt = ".lml",
                Filter = "LML-Files|*.lml",
                Multiselect = false
            };

            if (openDialog.ShowDialog() == true)
            {
                _editor = new CourseEditor(Path.GetDirectoryName(openDialog.FileName), CourseState.NotEmpty);
                DisplayDataFromEditor();
            }
        }

        private void SaveCourse()
        {
            SendDataToEditor();
            _editor.WriteDataOnFile();
        }

        private void CancelLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsOpenUnderCourse = false;
            Intermediary.CoursePage = null;
            PageManager.HidePages();
        }

        public void OpenLessonEditorPage(string filename)
        {
            Settings.Default.LessonInCourseFileName = filename;
            CloseLessonEditorPage();
            LessonEditorFrame.Source = new Uri("LessonEditorPage.xaml", UriKind.Relative);
        }

        public void CloseLessonEditorPage() =>
            LessonEditorFrame.Source = null;
        
        private void NewLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            NewCourse();

        private void OpenLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            OpenCourse();

        private void SaveLessonButton_OnClick(object sender, RoutedEventArgs e) =>
            SaveCourse();

        private void CourseEditoGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (AppManager.IsComboKeyDown(e, Key.LeftShift) || (AppManager.IsComboKeyDown(e, Key.RightShift)))
                    CloseLessonEditorPage();
                else
                    PageManager.HidePages();
            }

            else
            {
                if (AppManager.IsComboKeyDown(e, Key.LeftCtrl, Key.O) || AppManager.IsComboKeyDown(e, Key.RightCtrl, Key.O))
                    OpenCourse();

                else if (AppManager.IsComboKeyDown(e, Key.LeftCtrl, Key.N) || AppManager.IsComboKeyDown(e, Key.RightCtrl, Key.N))
                    NewCourse();

                else if (AppManager.IsComboKeyDown(e, Key.LeftCtrl, Key.S) || AppManager.IsComboKeyDown(e, Key.RightCtrl, Key.S))
                    SaveCourse();
            }
        }

        private string SelectedPath()
        {
            LessonListTextBox.SelectLine();
            var path = LessonListTextBox.SelectedText;
            var basePath = path.Contains(":\\") ? "" : _editor.FolderPath + '\\';

            return basePath + path;
        }

        private void EditLessonMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            OpenLessonEditorPage(SelectedPath());
        
        private void NewLessonMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            OpenLessonEditorPage("empty");
        
        private void MenuItem_OnClick(object sender, RoutedEventArgs e) =>
            System.Diagnostics.Process.Start("explorer.exe", $"select \"{SelectedPath()}\"");

        private void CopyPathMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            Clipboard.SetText(SelectedPath());

        private void DeleteFileMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var path = SelectedPath();
            LessonListTextBox.Text =
                LessonListTextBox.Text.Remove(LessonListTextBox.SelectionStart, LessonListTextBox.SelectionLength);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
