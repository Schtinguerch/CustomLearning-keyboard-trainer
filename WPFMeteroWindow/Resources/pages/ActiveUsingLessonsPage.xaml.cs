using Newtonsoft.Json;
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
using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Properties;

using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ActiveUsingLessonsPage.xaml
    /// </summary>
    public partial class ActiveUsingLessonsPage : Page
    {
        private List<List<string>> _recentCoursesData;

        public ActiveUsingLessonsPage()
        {
            InitializeComponent();
            ReinitializeRecentCoursesList();
        }

        private void ReinitializeRecentCoursesList()
        {
            ScrollListStackPanel.Children.Clear();
            _recentCoursesData = AppManager.JsonReadData<List<List<string>>>(Settings.Default.RecentCourcesPath);

            if (_recentCoursesData.Count > 0)
                EmptyListTextBox.Visibility = Visibility.Hidden;

            foreach (var course in _recentCoursesData)
                InsertNewCourse(course);
        }

        private void InsertNewCourse(List<string> courseData) =>
            ScrollListStackPanel.Children.Insert(0, new CourseItem()
            {
                CourseName = courseData[0],
                CourseType = courseData[1],
                LessonCount = int.Parse(courseData[2]),
                CoursePath = courseData[3],

                Margin = new Thickness(0, 20, 0, 0),
            });

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            PageManager.HidePages();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Opener.NewCourseViaExplorer();
            var newRecentCourcesData = JsonConvert.DeserializeObject<List<List<string>>>(
                File.ReadAllText(Settings.Default.RecentCourcesPath));

            if (newRecentCourcesData.Count == _recentCoursesData.Count)
                return;

            ReinitializeRecentCoursesList();
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollListStackPanel.Children.Clear();
            _recentCoursesData?.Clear();

            File.WriteAllText(Settings.Default.RecentCourcesPath, "[]");
            EmptyListTextBox.Visibility = Visibility.Visible;
        }

        private void wrapPanel_DragEnter(object sender, DragEventArgs e)
        {
            DragAndDropBackground.Opacity = 0.5d;

            EmptyListTextBox.VerticalAlignment = VerticalAlignment.Bottom;
            EmptyListTextBox.Text = Localization.uMenuOpenNewCourse;
            EmptyListTextBox.Visibility = Visibility.Visible;
            
        }

        private void wrapPanel_DragLeave(object sender, DragEventArgs e)
        {
            DragAndDropBackground.Opacity = 0d;

            EmptyListTextBox.VerticalAlignment = VerticalAlignment.Center;
            EmptyListTextBox.Text = Localization.uEmptyList;
            EmptyListTextBox.Visibility = Visibility.Hidden;
        }

        private void wrapPanel_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            
            var cources = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (cources == null)
                return;

            foreach (var course in cources)
                Opener.NewCourse(course, 0, true);

            ReinitializeRecentCoursesList();
            wrapPanel_DragLeave(null, null);
        }
    }
}
