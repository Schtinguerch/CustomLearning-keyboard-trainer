using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace WPFMeteroWindow
{
    public class WordPracticingCourseEditor
    {
        private string _folderPath;

        private string _data;

        private List<string> _allWords = new List<string>();

        public int NecessaryCPM { get; set; }

        public int MaxAcceptableMistakes { get; set; }

        public int WordCountInLesson { get; set; }

        public int RepeatsCount { get; set; }

        public string Data
        {
            get => _data;

            set
            {
                _data = Regex.Replace(value, "\n+", "\n");
                _allWords = value.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                _data = value;

                for (int i = 0; i < _allWords.Count; i++)
                    _allWords[i] = _allWords[i].Replace("\r", "");
            }
        }

        public WordPracticingCourseEditor(string courseFolderPath)
        {
            _folderPath = courseFolderPath;
        }

        public void LoadWords(string fileName)
        {
            if (!File.Exists(fileName))
            {
                LogManager.Log($"Open world list for course: \"{fileName}\" -> failed: file does not exist");
                MessageBox.Show(Resources.localizations.Resources.uOpenFileMessageError);
                return;
            }

            _allWords = File.ReadAllText(fileName).Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            _data = ListToString(_allWords, '\n');

            for (int i = 0; i < _allWords.Count; i++)
                _allWords[i] = _allWords[i].Replace("\r", "");
        }

        public void UnloadToCourse()
        {
            var newWordListOffset = WordCountInLesson / 4;
            var currentStart = 0;

            do
            {
                var lessonWords = SubList(currentStart, WordCountInLesson);
                var lessonName = $"{currentStart + 1}-{currentStart + WordCountInLesson}";
                var lessonText = ListToString(lessonWords, ' ');

                var fileName = $"{_folderPath}\\{lessonName}.lml";
                var editor = new LessonEditor(fileName)
                {
                    LessonText = Multiple(lessonText, RepeatsCount),
                    LessonName = lessonName,
                    NecessaryCPM = NecessaryCPM,
                    MaxAcceptableMistakes = MaxAcceptableMistakes
                };

                editor.WriteDataOnFile();
                Intermediary.CoursePage.Editor.Lessons.Add($"{lessonName}.lml");
                currentStart += newWordListOffset;
            } 
            while (currentStart < _allWords.Count);
        }

        private List<string> SubList(int startIndex, int itemsCount)
        {
            var subList = new List<string>();
            for (int i = startIndex; (i < _allWords.Count) && (i < startIndex + itemsCount); i++)
                subList.Add(_allWords[i]);

            return subList;
        }

        private string Multiple(string s, int multiples)
        {
            if (multiples <= 0) return "";

            var value = s;
            for (int i = 1; i < multiples; i++)
                value += s;

            return value;
        }

        private string ListToString(List<string> l, char splitter)
        {
            var list = "";

            if ((l != null) && (l.Count != 0))
                foreach (var item in l)
                    list += item + splitter;

            return list;
        }
    }
}
