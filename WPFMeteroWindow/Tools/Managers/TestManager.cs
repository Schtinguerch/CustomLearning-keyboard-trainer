using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using Microsoft.Win32;

namespace WPFMeteroWindow
{
    public enum TestWords
    {
        Top100,
        Top200,
        Top500,
        Top1000,
        Top2000,
        Top5000
    }

    public enum TestAdditional
    {
        None,
        Punctuation,
        Uppercase,
        Numbers,
        AllAdditionals
    }

    public class TestData
    {
        public int TestWordRangeIndex { get; set; }
        public int TestAdditionalModeIndex { get; set; }
        public int TestWordCount { get; set; }

        public List<string> RecentTestDictionaries { get; set; }
        public List<LessonStatistics> Results { get; set; }

        public int FirstWordIndex { get; set; }
        public int LastWordIndex { get; set; }
    }

    public static class TestManager
    {
        public static TestData Data { get; set; } = AppManager.JsonReadData<TestData>(Settings.Default.RecentTestDictionariesPath);

        private static Random _random = new Random();

        private static string _punctuationSymbols = "..........,,,,,,,,,,,,,::::;;;;;-----                      \"\"'''$@@*!!!!???+==#__((()))^%{}[]<>";

        private static List<string> _words = new List<string>();
        private static TestAdditional _testAdditional;

        public static string HeaderName() =>
            Localization.uTypingTest + 
            $": {Localization.uWords} [{_firstWordIndex - 1}; {_lastWordIndex - 1}]";

        private static string ToTitle(string word)
        {
            word = word.ToLower();
            var newWord = word[0].ToString().ToUpper();
            for (int i = 1; i < word.Length; i++)
                newWord += word[i];

            return newWord;
        }

        private static string PunctuationSymbol()
        {
            
            int index = _random.Next(0, _punctuationSymbols.Length);

            string value = "";
            char pnct = _punctuationSymbols[index];

            switch (pnct)
            {
                case ' ':
                    value += ' ';
                    break;

                case '.':
                case ',':
                case ';':
                case ':':
                case '\'':
                case '\"':
                case '?':
                case '!':
                case '^':
                case ')':
                case '}':
                case ']':
                case '>':
                case '%':
                    value += $"{pnct} ";
                    break;

                case '-':
                case '+':
                case '=':
                    value += $" {pnct} ";
                    break;

                case '#':
                case '@':
                case '_':
                case '(':
                case '{':
                case '[':
                case '<':
                    value += $" {pnct}";
                    break;
            }

            return value;
        }

        public static void LoadWordsViaExplorer()
        {
            var opener = new OpenFileDialog()
            {
                Multiselect = false,
                RestoreDirectory = true,
                Filter = "LML-files|*.lml",
            };

            if (opener.ShowDialog() == true)
                LoadWords(opener.FileName);
        }

        public static void LoadWords(string fileName)
        {
            if (File.Exists(fileName))
            {
                var heapData = File.ReadAllText(fileName);
                _words = heapData.Split(new[] { '\n' },
                    StringSplitOptions.RemoveEmptyEntries).ToList();

                for (int i = 0; i < _words.Count; i++)
                    _words[i] = _words[i].Replace("\r", "");

                Settings.Default.TestWordListPath = fileName;
                Settings.Default.Save();

                if (!Data.RecentTestDictionaries.Contains(fileName))
                    Data.RecentTestDictionaries.Add(fileName);

                LogManager.Log($"Open world list for test: \"{fileName}\" -> success");
            }

            else
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Open world list for test: \"{fileName}\" -> failed: file does not exist");
            }
        }

        private static int _firstWordIndex, _lastWordIndex;
        private static void FormUpTheLesson(int firstWordIndex, int lastWordIndex, TestAdditional additional)
        {
            var lesson = "";
            lastWordIndex = Math.Min(lastWordIndex, _words.Count);

            var generatorFunctions = new Func<int, string>[]
            {
                index => _words[index] + ' ',
                index => _words[index] + ' ' + ((index / 2 % 3 == 0) ? _random.Next(0, 10000).ToString() + ' ' : ""),
                index => _words[index] + PunctuationSymbol(),
                index => ToTitle(_words[index]) + ' ',
                index => ToTitle(_words[index]) + PunctuationSymbol() +
                         ((index / 2 % 5 == 0) ? _random.Next(0, 10000).ToString() + ' ' : ""),
            };

            for (int i = 0; i < Data.TestWordCount; i++)
            {
                int index = _random.Next(firstWordIndex, lastWordIndex);
                int function = (int) additional;

                lesson += generatorFunctions[function](index);
            }

            _firstWordIndex = firstWordIndex + 1;
            _lastWordIndex = lastWordIndex + 1;
            LessonManager.LoadTest(Regex.Replace(lesson, "\n", " ", RegexOptions.Multiline));
        }

        public static void StartTest(TestWords wordCount, TestAdditional additional)
        {
            int firstWordIndex = 0;
            var wordLengths = new int[] { 100, 200, 500, 1000, 2000, 5000 };

            int lastWordIndex = wordLengths[(int)wordCount];
            StartTest(firstWordIndex, lastWordIndex, additional);
        }

        public static void StartTest(int firstWordIndex, int lastWordIndex, TestAdditional additional)
        {
            if ((_words == null) || (_words.Count == 0))
                LoadWords(Settings.Default.TestWordListPath);

            Data.FirstWordIndex = firstWordIndex;
            Data.LastWordIndex = lastWordIndex;
            _testAdditional = additional;

            LessonManager.ErrorInput = "";
            FormUpTheLesson(firstWordIndex, lastWordIndex, additional);
        }

        public static void RestartTest()
        {
            LessonManager.ErrorInput = "";
            FormUpTheLesson(Data.FirstWordIndex, Data.LastWordIndex, _testAdditional);
        }
    }
}
