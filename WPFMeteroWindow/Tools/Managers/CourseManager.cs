using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using LmlLibrary;
using WPFMeteroWindow.Properties;
using Newtonsoft.Json;

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
                    Settings.Default.CourseLessonNumber = value;
                    Settings.Default.Save();
                
                    StatisticsManager.ReloadTimer();
                    if (Settings.Default.IsCourseOpened)
                    {
                        int choosenLesson = 0;

                        if (Settings.Default.CourseLessonNumber == value && Settings.Default.IsDictionary)
                            choosenLesson = new Random().Next(0, LessonsCount);
                        else
                            choosenLesson = value;

                        LessonManager.LoadLesson(Lessons[choosenLesson]);

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
            var isDictionary = reader.GetAllData().Substring(0, 11) == "#dictionary";

            var courseName = reader.GetString("Course>Name");
            if (isDictionary)
                courseType = "#dictionary";

            if (!skipUiLoadng)
            {
                Lessons = lessons;
                LessonsCount = lessons.Count;

                LessonManager.LoadLesson(Lessons[Settings.Default.CourseLessonNumber]);
                Intermediary.App.LoadCourseOrLessonButton.ContextMenu = CourseContextMenu();

                Settings.Default.CourseName = courseName;
                Settings.Default.IsDictionary = isDictionary;

                Settings.Default.LoadedLessonFile = Lessons[Settings.Default.CourseLessonNumber];
                Settings.Default.LoadedCourseFile = filename;
                Settings.Default.Save();

                Intermediary.App.NextLessonButton.Visibility = Visibility.Visible;
                Intermediary.App.PrevLessonButton.Visibility = Visibility.Visible;

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
            var recentCources = JsonConvert.DeserializeObject<List<List<string>>>(
                File.ReadAllText(Settings.Default.RecentCourcesPath));

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

        private static ContextMenu CourseContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.MaxHeight = 300d;
            contextMenu.FontFamily = new FontFamily(Settings.Default.SummaryFont);

            foreach (var lesson in Lessons)
            {
                var reader = new Lml(lesson, Lml.Open.FromFile);
                var lessonName = reader.GetString("Lesson>Name");

                var newMenuItem = new MenuItem { Header = lessonName };
                newMenuItem.Click += (s, e) =>
                   CurrentLessonIndex = contextMenu.Items.IndexOf(s as MenuItem);

                contextMenu.Items.Add(newMenuItem);
            }

            return contextMenu;
        }
    }
}