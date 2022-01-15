using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using LmlLibrary;
using WPFMeteroWindow.Properties;
using Newtonsoft.Json;
using WPFMeteroWindow.Resources.pages;
using System.Threading.Tasks;

namespace WPFMeteroWindow
{
    public class CourseManager
    {
        public static List<string> Lessons { get; private set; }
        
        public static int LessonsCount { get; private set; }
        
        public static int CurrentLessonIndex
        {
            get => Settings.Default.CourseLessonNumber;
            set
            {
                try
                {
                    StatisticsManager.ReloadTimer();
                    if (Settings.Default.IsCourseOpened)
                    {
                        int choosenLesson = 0;

                        if (Settings.Default.CourseLessonNumber == value && Settings.Default.IsDictionary)
                            choosenLesson = new Random().Next(0, LessonsCount);
                        else
                            choosenLesson = value;

                        LessonManager.LoadLesson(Lessons[choosenLesson]);

                        Settings.Default.CourseLessonNumber = value;
                        Settings.Default.LessonInCourseFileName = Lessons[choosenLesson];
                        Settings.Default.Save();
                    }
                    else
                        LessonManager.LoadLesson(Settings.Default.LoadedLessonFile);

                    int lessonsCount = (Lessons != null) ? Lessons.Count : 0;
                    
                    Intermediary.App.PrevLessonButton.Visibility 
                        = (value == 0) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                    Intermediary.App.NextLessonButton.Visibility 
                        = (value == lessonsCount - 1) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                }
                
                catch { }
            }
        }

        public static void LoadCourse(string filename, bool skipUiLoadng = false)
        {
            var courseType = "";
            if (filename.Contains(".lml"))
            {
                while (!File.Exists(filename + "\\CourseLessons.lml"))
                    filename = new DirectoryInfo(filename).Parent.FullName;
            }

            var reader = new Lml(filename + "\\CourseLessons.lml", Lml.Open.FromFile);
            var fullName = new DirectoryInfo(filename).FullName;

            var lessons = AppManager.GetFileList(reader.GetArray("Course>LessonList"), fullName);
            var isDictionary = reader.GetAllData().Contains("#dictionary");

            var courseName = reader.GetString("Course>Name");
            if (isDictionary)
                courseType = "#dictionary";

            if (!skipUiLoadng)
            {
                if (StatisticsManager.CourseStatistics != null)
                {
                    File.WriteAllText(
                        Settings.Default.LoadedCourseFile + "\\statistics.json",
                        JsonConvert.SerializeObject(
                            StatisticsManager.CourseStatistics,
                            Formatting.Indented));
                }

                Lessons = lessons;
                LessonsCount = lessons.Count;

                LessonManager.LoadLesson(Lessons[Settings.Default.CourseLessonNumber]);
                Intermediary.App.LoadCourseOrLessonButton.ContextMenu = NewContextMenu();

                Settings.Default.CourseName = courseName;
                Settings.Default.IsDictionary = isDictionary;

                Settings.Default.LoadedLessonFile = Lessons[Settings.Default.CourseLessonNumber];
                Settings.Default.LoadedCourseFile = filename;
                Settings.Default.Save();

                Intermediary.App.NextLessonButton.Visibility = Visibility.Visible;
                Intermediary.App.PrevLessonButton.Visibility = Visibility.Visible;

                StatisticsManager.CourseStatistics = AppManager.JsonReadData<List<LessonStatistics>>(filename + "\\statistics.json");
                CurrentLessonIndex = Settings.Default.CourseLessonNumber;
            }

            WriteDataToJson(
                name: courseName,
                type: courseType,
                lessonCount: lessons.Count,
                filename: filename);
        }

        public static void WriteDataToJson(string name, string type, int lessonCount, string filename)
        {
            var hasTheSameCourse = false;
            var recentCources = AppManager.JsonReadData<List<List<string>>>(Settings.Default.RecentCourcesPath);

            foreach (var item in recentCources)
            {
                if (filename == item[3])
                {
                    hasTheSameCourse = true;
                    break;
                }
            }

            if (!hasTheSameCourse)
                recentCources.Add(new List<string>()
                {
                    name,
                    type,
                    lessonCount.ToString(),
                    filename,
                });

            File.WriteAllText(
                Settings.Default.RecentCourcesPath,
                JsonConvert.SerializeObject(recentCources, Formatting.Indented));
        }

        public static ContextMenu NewContextMenu(bool isFiction = false)
        {
            var contextMenu = new ContextMenu()
            {
                Margin = new Thickness(0d),
                Padding = new Thickness(0d),
                BorderBrush =
                new BrushConverter().ConvertFromString(Settings.Default.SecondBackground) as SolidColorBrush,
                HasDropShadow = false,
            };

            if (!isFiction)
            {
                contextMenu.Items.Add(new ChooseLessonMenu(Lessons));
            }

            else
            {
                contextMenu.Items.Add(new ChooseLessonMenu(new List<string>()));
            }

            return contextMenu;
        }
    }
}