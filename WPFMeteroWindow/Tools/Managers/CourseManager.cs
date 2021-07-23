using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using LmlLibrary;
using WPFMeteroWindow.Properties;

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
                        LessonManager.LoadLesson(Lessons[value]);
                    else
                        LessonManager.LoadLesson(Settings.Default.LoadedLessonFile);
                    
                    Intermediary.App.PrevLessonButton.Visibility 
                        = (value == 0) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                    Intermediary.App.NextLessonButton.Visibility 
                        = (value == Lessons.Count - 1) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                }
                
                catch { }
            }
        }
        
        public static void LoadCourse(string filename)
        {
            if (filename.Contains(".lml"))
            {
                while (!File.Exists(filename + "\\CourseLessons.lml"))
                    filename = new DirectoryInfo(filename).Parent.FullName;
            }
            
            var reader = new Lml(filename + "\\CourseLessons.lml", Lml.Open.FromFile);
            var lessonFiles =  AppManager.GetFileList(reader.GetArray("Course>LessonList"));

            for (int i = 0; i < lessonFiles.Count; i++)
                lessonFiles[i] = new DirectoryInfo(filename).FullName + "\\" + lessonFiles[i];

            Lessons = lessonFiles;
            LessonsCount = Lessons.Count;

            LessonManager.LoadLesson(lessonFiles[Settings.Default.CourseLessonNumber]);
            
            Settings.Default.LoadedLessonFile = lessonFiles[Settings.Default.CourseLessonNumber];
            Settings.Default.LoadedCourseFile = filename;
            Settings.Default.CourseName = reader.GetString("Course>Name");
            Settings.Default.Save();

            Intermediary.App.NextLessonButton.Visibility = Visibility.Visible;
            Intermediary.App.PrevLessonButton.Visibility = Visibility.Visible;

            CurrentLessonIndex = Settings.Default.CourseLessonNumber;
        }
    }
}