﻿using System;
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
        public CourseEditor Editor => _editor;

        public CourseEditorPage()
        {
            InitializeComponent();
            HeaderNameTextBlock.Text = $"{Localization.uName}, {Localization.uAuthor}";

            Settings.Default.LessonInCourseFileName = "empty";
            Settings.Default.IsOpenUnderCourse = true;
            Intermediary.CoursePage = this;
            LessonListTextBox.Focus();

            Intermediary.RichPresentManager.Update("Course editor", "Editing course...", "");

            if (string.IsNullOrWhiteSpace(Settings.Default.LoadedCourseFile))
            {
                NewCourse();
                return;
            }

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

        public void DisplayDataFromEditor()
        {
            CourseNameTextBox.Text = _editor.CourseName;
            AuthorTextBox.Text = _editor.Author.Name;
            LessonListTextBox.Text = _editor.ListToString(_editor.Lessons, 0);
            ReferencesListTextBox.Text = _editor.ListToString(_editor.Author.References, 0);
            EditorTitleTextBox.Text = $"{_editor.CourseName}.lml - {Localization.uCourseEditor}";

            TypeComboBox.SelectedIndex = (int)_editor.CourseType;
        }

        private void SendDataToEditor()
        {
            _editor.CourseName = CourseNameTextBox.Text;
            _editor.Lessons = _editor.StringToList(LessonListTextBox.Text);
            _editor.CourseType = (CourseType) TypeComboBox.SelectedIndex;

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
            if (string.IsNullOrEmpty(filename))
                return;

            Settings.Default.LessonInCourseFileName = filename;
            CloseLessonEditorPage();
            LessonEditorFrame.Source = new Uri("LessonEditorPage.xaml", UriKind.Relative);
        }

        public void CloseLessonEditorPage()
        {
            LessonEditorFrame.Source = null;
            Intermediary.RichPresentManager.Update("Course editor", "Editing course...", "");
        }
            
        
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
                e.Handled = true;

                if (AppManager.IsComboKeyDown(e, Key.LeftShift) || (AppManager.IsComboKeyDown(e, Key.RightShift)))
                    CloseLessonEditorPage();
                else
                    PageManager.HidePages();
            }

            else
            {
                if (AppManager.IsComboKeyDown(e, Key.LeftCtrl, Key.O) ||
                    AppManager.IsComboKeyDown(e, Key.RightCtrl, Key.O))
                {
                    e.Handled = true;
                    OpenCourse();
                }
                    

                else if (AppManager.IsComboKeyDown(e, Key.LeftCtrl, Key.N) ||
                         AppManager.IsComboKeyDown(e, Key.RightCtrl, Key.N))
                {
                    e.Handled = true;
                    NewCourse();
                }
                    

                else if (AppManager.IsComboKeyDown(e, Key.LeftCtrl, Key.S) ||
                         AppManager.IsComboKeyDown(e, Key.RightCtrl, Key.S))
                {
                    e.Handled = true;
                    SaveCourse();
                }
                    
            }
        }

        private string SelectedPath()
        {
            try
            {
                LessonListTextBox.SelectLine();
                var path = LessonListTextBox.SelectedText;
                var basePath = path.Contains(":\\") ? "" : _editor.FolderPath + '\\';

                return basePath + path;
            }
            
            catch
            {
                Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uCannotWorkWithEmptyString}");
                return "";
            }
        }

        private void ExploreFile(string path)
        {
            if (!File.Exists(path) || string.IsNullOrEmpty(path))
            {
                LogManager.Log($"Show file in explorer: \"{path}\" -> failed, file does not exist");
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
            LogManager.Log($"Show file in explorer: \"{path}\" -> success");
        }

        private void EditLessonMenuItem_OnClick(object sender, RoutedEventArgs e) =>
            OpenLessonEditorPage(SelectedPath());
        
        private void NewLessonMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_editor.FolderPath))
                OpenLessonEditorPage("empty");
            else
                Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uCannotAddLessonToEmptyCourse}");
        }
            

        private void MenuItem_OnClick(object sender, RoutedEventArgs e) =>
            ExploreFile(SelectedPath());

        private void CopyPathMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var path = SelectedPath();
            if (string.IsNullOrEmpty(path))
                return;

            Clipboard.SetText(path);
        }
            

        private void DeleteFileMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var path = SelectedPath();
            if (string.IsNullOrEmpty(path))
                return;

            LessonListTextBox.Text =
                LessonListTextBox.Text.Remove(LessonListTextBox.SelectionStart, LessonListTextBox.SelectionLength);

            if (File.Exists(path))
                File.Delete(path);
        }

        private void LoadWordsButton_OnClick(object sender, RoutedEventArgs e)
        {
            CloseLessonEditorPage();
            NewCourse();

            CourseNameTextBox.Text = "New word practicing";
            AuthorTextBox.Text = "CustomLearning";

            SaveCourse();
            LessonEditorFrame.Source = new Uri("WordPracticingEditorPage.xaml", UriKind.Relative);
        }

        private void FormLayoutEducation_OnClick(object sender, RoutedEventArgs e)
        { 
            CloseLessonEditorPage();
            NewCourse();

            //TODO: call the course maker
        }

        private void HtmlParsing_OnClick(object sender, RoutedEventArgs e)
        {
            CloseLessonEditorPage();
            NewCourse();

            //TODO: make HTML-parser
        }
    }
}
