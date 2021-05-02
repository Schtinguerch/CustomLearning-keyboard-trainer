using System;
using System.IO;
using System.Text.RegularExpressions;
using LmlLibrary;
using Microsoft.Win32;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public class LessonEditor
    {
        private string _filePath;

        private bool _isNewLesson;
        
        public int NecessaryCPM { get; set; }
        
        public int MaxAcceptableMistakes { get; set; }
        
        public string LessonName { get; set; }
        
        public string LessonText { get; set; }
        
        public LessonEditor() =>
            _isNewLesson = true;

        public LessonEditor(string filePath)
        {
            _filePath = filePath;
            _isNewLesson = false;

            if (File.Exists(filePath))
            {
                var reader = new Lml(filePath, Lml.Open.FromFile);

                LessonName = reader.GetString("Lesson>Name");
                LessonText = Regex.Replace(reader.GetString("Lesson>Text"), "\\s+", " ").ToBeCorrected();

                NecessaryCPM = reader.GetInt("Lesson>NecessaryCPM");
                MaxAcceptableMistakes = reader.GetInt("Lesson>MaximumMistakes");
            }
        }

        public void WriteDataOnFile()
        {
            if (!_isNewLesson)
            {
                if (!string.IsNullOrEmpty(_filePath))
                    File.WriteAllText(_filePath, ToLml());
                else 
                    throw new NotImplementedException("Empty data");
            }

            else
            {
                var writer = new SaveFileDialog
                {
                    Filter = "LML-files | *.lml", 
                    DefaultExt = ".lml"
                };

                if (writer.ShowDialog() == true)
                {
                    _isNewLesson = false;
                    _filePath = writer.FileName;
                    
                    File.WriteAllText(_filePath, ToLml());

                    if (Settings.Default.IsOpenUnderCourse)
                        Intermediary.CoursePage.AddNewLesson(_filePath);

                    Intermediary.LessonPage.EditorTitleTextBox.Text = $"{LessonName} - {Localization.uLessonEditor}";
                }
            }
        }

        private string ToLml()
        {
            LessonText = Regex.Replace(LessonText, "\n+|\\s+", " ");
            LessonText = LessonText.ToLmlFormat();
            
            var data = "<<Lesson:\n";
            data += $"    <Name {LessonName}>>\n";
            data += $"    <NecessaryCPM {NecessaryCPM}>>\n";
            data += $"    <MaximumMistakes {MaxAcceptableMistakes}>>\n\n";
            data += $"    <Text {LessonText}>>\n";
            data += ":Lesson>>";

            return data;
        }

        public override string ToString() => 
            _filePath;
    }
}