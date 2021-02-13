using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WPFMeteroWindow.Properties;
using LmlLibrary;
using Microsoft.Win32;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace WPFMeteroWindow.Resources.pages
{
    public partial class CourseLoaderPage : Page
    {
        public CourseLoaderPage()
        {
            InitializeComponent();
            CourseTextBox.Focus();
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
                        Actions.TheWindow.HideSettingGrid();
                    }
                        
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:\n" + ex.Message);
                }
            }
        }

        private void LoadCourseData(string filename)
        {
            var reader = new Lml(filename, Lml.Open.FromFile);
            var lessonFiles = Actions.GetFileList(reader.GetArray("Course>LessonList"));
            _courseName = reader.GetString("Course>Name");
            _courseRef = filename;

            LessonsComboBox.Items.Clear();
            foreach (var lessonFile in lessonFiles)
                LessonsComboBox.Items.Add(Regex.Replace(lessonFile, "(^|^.*?)\\\\|[.]lml", ""));

            LessonsComboBox.Focus();
        }
    }
}
