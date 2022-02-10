using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WPFMeteroWindow.Properties;
using LmlLibrary;
using System.Windows.Threading;

namespace WPFMeteroWindow.Resources.pages.WelcomePageSubPages
{
    /// <summary>
    /// Логика взаимодействия для CourseSetup.xaml
    /// </summary>
    public partial class CourseSetup : Page, IRequstable
    {
        private List<string> _coursePaths = new List<string>();
        private List<string> _lessons;

        public CourseSetup()
        {
            InitializeComponent(); 
            LoadCourses();
        }

        public bool RequestVadid()
        {
            if (CourseComboBox.SelectedItem == null)
            {
                CourseComboBox.BorderBrush = Brushes.Red;
                return false;
            }

            if (LessonComboBox.SelectedItem == null)
            {
                LessonComboBox.BorderBrush = Brushes.Red;
                return false;
            }

            CourseComboBox.BorderBrush 
                = LessonComboBox.BorderBrush 
                = new SolidColorBrush(Color.FromArgb(255, 18, 18, 18));

            Opener.NewCourse(_coursePaths[CourseComboBox.SelectedIndex], LessonComboBox.SelectedIndex);
            return true;
        }

        private void CourseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var courseFile = _coursePaths[CourseComboBox.SelectedIndex];
            var reader = new Lml(courseFile, Lml.Open.FromFile);

            _lessons = AppManager.GetFileList(reader.GetArray("Course>LessonList"), new DirectoryInfo(courseFile).Parent.FullName);
            LoadLessonsAsync();
        }

        private void LoadCourses()
        {
            var folder = "JustLessons\\Schtinguêrch's Challenge cources";
            var courses = Directory.GetDirectories(folder);

            foreach (var course in courses)
            {
                var path = $"{course}\\CourseLessons.lml";

                if (!File.Exists(path))
                    continue;

                var reader = new Lml(path, Lml.Open.FromFile);
                var header = reader.GetString("Course>Name");

                _coursePaths.Add(path);
                CourseComboBox.Items.Add(header);
            }

            CourseComboBox.SelectedIndex = 0;
        }

        private delegate void LessonLoadMethod(int index);
        private void LoadLessonsAsync()
        {
            LessonComboBox.Items.Clear();
            if (_lessons == null || _lessons.Count == 0)
                return;

            _count = _lessons.Count;
            LessonComboBox.IsEnabled = false;

            LoadingStatusTextBox.Opacity = 1d;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new LessonLoadMethod(LoadLessonViaIndex), 0);
        }

        private int _count;
        private void LoadLessonViaIndex(int index)
        {
            AddLesson(_lessons[index]);
            if (index == _count - 1)
            {
                if (LessonComboBox.Items.Count > 0)
                    LessonComboBox.SelectedIndex = 0;

                LessonComboBox.IsEnabled = true;
                LoadingStatusTextBox.Opacity = 0d;
                return;
            }

            index += 1;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new LessonLoadMethod(LoadLessonViaIndex), index);
        }

        private void AddLesson(string filename)
        {
            var lmlReader = new Lml(filename, Lml.Open.FromFile);
            var header = lmlReader.GetString("Lesson>Name");

            LessonComboBox.Items.Add(header);
        }
    }
}
