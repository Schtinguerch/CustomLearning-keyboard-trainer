﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using LmlLibrary;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    public partial class CourseLoaderPage : Page
    {
        public CourseLoaderPage()
        {
            InitializeComponent();
            CourseTextBox.Focus();

            Intermediary.RichPresentManager.Update("Command line", "Choosing a course to passing...", "");
        }

        private string _courseName;

        private string _courseRef;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new FolderBrowserDialog();

            if (openDialog.ShowDialog() == DialogResult.OK)
                LoadCourseData(openDialog.SelectedPath  + "\\CourseLessons.lml");
        }

        private void CourseLoaderPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (CourseTextBox.IsFocused)
                        LoadCourseData(CourseTextBox.Text + "\\CourseLessons.lml");

                    else if (LessonsComboBox.IsFocused)
                    {
                        Opener.NewCourse(_courseRef,
                            (LessonsComboBox.SelectedIndex == -1) ? 0 : LessonsComboBox.SelectedIndex);
                        PageManager.HidePages();
                    }
                        
                }
                catch
                {
                    MessageBox.Show(Localization.uOpenFileMessageError);
                    LogManager.Log($"Open course: \"{CourseTextBox.Text}\" -> failed, file does not exist");
                }
            }

            else if (e.Key == Key.Escape)
                PageManager.HidePages();
        }

        private void LoadCourseData(string filename)
        {
            if (!File.Exists(filename))
                throw new NullReferenceException();

            var reader = new Lml(filename, Lml.Open.FromFile);
            var lessonFiles = AppManager.GetFileList(reader.GetArray("Course>LessonList"));
            _courseName = reader.GetString("Course>Name");
            _courseRef = filename;

            LessonsComboBox.Items.Clear();
            foreach (var lessonFile in lessonFiles)
                LessonsComboBox.Items.Add(Regex.Replace(lessonFile, "(^|^.*?)\\\\|[.]lml", ""));

            LessonsComboBox.Focus();
        }
    }
}
