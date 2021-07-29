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
    
    public static class TestManager
    {
        private static int _wordCount = Settings.Default.TestWordCount;

        private static string _punctuationSymbols = "..........,,,,,,,,,,,,,::::;;;;;-----                      \"\"'''$@@*!!!!???+==#__((()))^%{}[]<>";

        public static int WordCount
        {
            get => _wordCount;
            set
            {
                if (value > 0)
                {
                    _wordCount = value;
                    Settings.Default.TestWordCount = value;
                    Settings.Default.Save();
                }
            }
        }

        private static List<string> _words = new List<string>();

        private static TestAdditional _testAdditional;

        private static int _firstWordIndex;

        private static int _lastWordIndex;

        private static string ToTitle(string word)
        {
            word = word.ToLower();
            var newWord = word[0].ToString().ToUpper();
            for (int i = 1; i < word.Length; i++)
                newWord += word[i];

            return newWord;
        }

        private static string PunctuationSymbol(int seed)
        {
            var rand = new Random(seed * DateTime.Now.Millisecond);
            int index = rand.Next(0, _punctuationSymbols.Length);

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

                LogManager.Log($"Open world list for test: \"{fileName}\" -> success");
            }

            else
            {
                MessageBox.Show(Localization.uOpenFileMessageError);
                LogManager.Log($"Open world list for test: \"{fileName}\" -> failed: file does not exist");
            }
        }

        private static void FormUpTheLesson(int firstWordIndex, int lastWordIndex, TestAdditional additional)
        {
            var randomizer = new Random(DateTime.Now.Millisecond);
            var lesson = "";
            
            for (int i = 0; i < _wordCount; i++)
            {
                int index = randomizer.Next(firstWordIndex, lastWordIndex);
                switch (additional)
                {
                    case TestAdditional.None:
                        lesson += _words[index] + ' ';
                        break;
                    
                    case TestAdditional.Numbers:
                        lesson += _words[index] + ' ' + ((index / 2 % 3 == 0) ? randomizer.Next(0, 10000).ToString() + ' ' : "");
                        break;
                    
                    case TestAdditional.Punctuation:
                        lesson += _words[index] + PunctuationSymbol(i);
                        break;
                    
                    case TestAdditional.Uppercase:
                        lesson += ToTitle(_words[index]) + ' ';
                        break;
                    
                    case TestAdditional.AllAdditionals:
                        lesson += ToTitle(_words[index]) + PunctuationSymbol(i) +
                                  ((index / 2 % 5 == 0) ? randomizer.Next(0, 10000).ToString() + ' ' : "");
                        break;
                }
            }
            
            LessonManager.LoadTest(Regex.Replace(lesson, "\n", " ", RegexOptions.Multiline));
        }

        public static void StartTest(TestWords wordCount, TestAdditional additional)
        {
            int
                firstWordIndex = 0,
                lastWordIndex = 0;
            
            switch (wordCount)
            {
                case TestWords.Top100:
                    lastWordIndex = 100;
                    break;
                
                case TestWords.Top200:
                    lastWordIndex = 200;
                    break;
                
                case TestWords.Top500:
                    lastWordIndex = 500;
                    break;
                
                case TestWords.Top1000:
                    lastWordIndex = 1000;
                    break;
                
                case TestWords.Top2000:
                    lastWordIndex = 2000;
                    break;
                
                case TestWords.Top5000:
                    lastWordIndex = 5000;
                    break;
            }

            StartTest(firstWordIndex, lastWordIndex, additional);
        }

        public static void StartTest(int firstWordIndex, int lastWordIndex, TestAdditional additional)
        {
            if ((_words.Count == null) || (_words.Count == 0))
                LoadWords(Settings.Default.TestWordListPath);

            _firstWordIndex = firstWordIndex;
            _lastWordIndex = lastWordIndex;
            _testAdditional = additional;

            LessonManager.ErrorInput = "";
            FormUpTheLesson(firstWordIndex, lastWordIndex, additional);
        }

        public static void RestartTest()
        {
            LessonManager.ErrorInput = "";
            FormUpTheLesson(_firstWordIndex, _lastWordIndex, _testAdditional);
        }
    }
}
