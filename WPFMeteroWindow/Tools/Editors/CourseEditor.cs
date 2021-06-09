using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LmlLibrary;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public struct AuthorData
    {
        public string Name;
        public List<string> References;
    }

    public enum CourseState
    {
        Empty,
        NotEmpty,
    }
    
    public class CourseEditor
    {
        private bool _isNewCourse;

        private string _folderPath;

        public string CourseName { get; set; }

        public List<string> Lessons { get; set; }

        public AuthorData Author { get; set; }

        public CourseEditor()
        {
            Lessons = new List<string>();
            _isNewCourse = true;

            Author = new AuthorData()
            {
                Name = "iHuman",
                References = null,
            };
        }

        public CourseEditor(string folderPath, CourseState state)
        {
            _folderPath = folderPath;
            _isNewCourse = false;

            if (state == CourseState.NotEmpty)
            {
                var reader = new Lml(folderPath + "\\CourseLessons.lml", Lml.Open.FromFile);

                CourseName = reader.GetString("Course>Name");
                Lessons = AppManager.GetFileList(reader.GetArray("Course>LessonList"));

                Author = new AuthorData()
                {
                    Name = reader.GetString("Author>Name"),
                    References = reader.GetArray("Author>References").ToList(),
                };
            }
        }

        public void WriteDataOnFile()
        {
            if (!_isNewCourse)
            {
                if (string.IsNullOrEmpty(_folderPath))
                    throw new NotImplementedException("Empty data");
                
                File.WriteAllText(_folderPath + "\\CourseLessons.lml", ToLml());
                Intermediary.CoursePage.EditorTitleTextBox.Text = $"{CourseName} - {Localization.uCourseEditor}";
            }

            else
            {
                var writer = new FolderBrowserDialog();
                if (writer.ShowDialog() != DialogResult.OK) return;

                _isNewCourse = false;
                _folderPath = writer.SelectedPath;

                File.WriteAllText(_folderPath + "\\CourseLessons.lml", ToLml());
                Intermediary.CoursePage.EditorTitleTextBox.Text = $"{CourseName} - {Localization.uCourseEditor}";
            }
        }

        private string ToLml()
        {
            var data = "<<Course:\n";
            data += $"    <Name {CourseName}>>\n";
            data +=  "    <LessonList {\n";
            data += ListToString(Lessons, 8);
            data +=  "    }>>\n";
            data += $":Course>>\n\n";

            data += "<<Author:\n";
            data += $"    <Name {Author.Name}>>\n";
            data +=  "    <References {\n";
            data += ListToString(Author.References, 8);
            data +=  "    }>>\n";
            data += ":Author>>";

            return data;
        }

        public string ListToString(List<string> l, int spaceOffset)
        {
            var list = "";
            var offset = "";

            for (int i = 0; i < spaceOffset; i++)
                offset += ' ';

            if ((l != null) && (l.Count != 0))
                foreach (var item in l)
                    list += offset + item + '\n';

            return list;
        }

        public string FolderPath => _folderPath;

        public List<string> StringToList(string s) => 
            s.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}