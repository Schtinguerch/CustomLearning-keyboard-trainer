using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using WPFMeteroWindow.Properties;
using Microsoft.Win32;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для TypingTestParametersPage.xaml
    /// </summary>
    public partial class TypingTestParametersPage : Page
    {
        private readonly string[] _lastWordIndexes = new string[]
        {
            "100", "200", "500", "1000", "2000", "5000"
        };

        public TypingTestParametersPage()
        {
            InitializeComponent();

            if (TestManager.Data.TestWordCount == 0)
                TestManager.Data.TestWordCount = 24;

            WordCountTextBox.Text = TestManager.Data.TestWordCount.ToString();
            FirstWordIndexTextBox.Text = TestManager.Data.FirstWordIndex.ToString();
            LastWordIndexTextBox.Text = TestManager.Data.LastWordIndex.ToString();

            WordsCountComboBox.SelectedIndex = TestManager.Data.TestWordRangeIndex;
            AdditionalModesComboBox.SelectedIndex = TestManager.Data.TestAdditionalModeIndex;

            if (TestManager.Data.RecentTestDictionaries == null)
            {
                TestManager.Data.RecentTestDictionaries = new List<string>();
                TestManager.Data.RecentTestDictionaries.Add(Settings.Default.TestWordListPath);
                return;
            }

            int index = TestManager.Data.RecentTestDictionaries.Count + 1;
            foreach (var dictionary in TestManager.Data.RecentTestDictionaries)
            {
                index -= 1;
                TestDictionaryFilenameComboBox.Items.Insert(1, dictionary);

                if (dictionary == Settings.Default.TestWordListPath)
                    TestDictionaryFilenameComboBox.SelectedIndex = index;
            }
        }

        private void CancelLessonButton_Click(object sender, RoutedEventArgs e) =>
            PageManager.HidePages();

        private void ApplySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetStandardBorderBrush(
                FirstWordIndexTextBox, 
                LastWordIndexTextBox, 
                WordCountTextBox,
                TestDictionaryFilenameComboBox);

            if (TestDictionaryFilenameComboBox.SelectedItem == null)
            {
                InvalidDataHighlighting(TestDictionaryFilenameComboBox);
                return;
            }

            var successfulReading = int.TryParse(FirstWordIndexTextBox.Text, out int firstWordIndex);
            if (!successfulReading || firstWordIndex <= 0)
            {
                InvalidDataHighlighting(FirstWordIndexTextBox);
                return;
            }

            successfulReading = int.TryParse(LastWordIndexTextBox.Text, out int lastWordIndex);
            if (!successfulReading || lastWordIndex <= 0)
            {
                InvalidDataHighlighting(LastWordIndexTextBox);
                return;
            }

            successfulReading = int.TryParse(WordCountTextBox.Text, out int wordCount);
            if (!successfulReading || wordCount <= 0)
            {
                InvalidDataHighlighting(WordCountTextBox);
                return;
            }

            TestManager.Data.FirstWordIndex = firstWordIndex;
            TestManager.Data.LastWordIndex = lastWordIndex;
            TestManager.Data.TestWordRangeIndex = WordsCountComboBox.SelectedIndex;
            TestManager.Data.TestAdditionalModeIndex = AdditionalModesComboBox.SelectedIndex;
            TestManager.Data.TestWordCount = wordCount;

            TestManager.LoadWords(TestDictionaryFilenameComboBox.SelectedItem.ToString());

            if (WordsCountComboBox.SelectedIndex == WordsCountComboBox.Items.Count - 1)
                TestManager.StartTest(firstWordIndex, lastWordIndex, (TestAdditional)TestManager.Data.TestAdditionalModeIndex);
            else
                TestManager.StartTest((TestWords)TestManager.Data.TestWordRangeIndex, (TestAdditional)TestManager.Data.TestAdditionalModeIndex);

            PageManager.HidePages();
        }

        private void WordsCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WordsCountComboBox.SelectedIndex == _lastWordIndexes.Length)
                return;

            FirstWordIndexTextBox.Text = "1";
            LastWordIndexTextBox.Text = _lastWordIndexes[WordsCountComboBox.SelectedIndex];
        }

        private void InvalidDataHighlighting(UIElement control)
        {
            dynamic item = control;

            if (item is TextBox) item = item as TextBox;
            else if (item is ComboBox) item = item as ComboBox;
            else return;

            item.BorderBrush = 
                (SolidColorBrush)new BrushConverter()
                .ConvertFromString(Settings.Default.KeyboardErrorHighlightColor);

            Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uInvalidDataInput}");
        }

        private void SetStandardBorderBrush(params UIElement[] controls)
        {
            foreach (var control in controls)
            {
                dynamic item = control;

                if (item is TextBox) item = item as TextBox;
                else if (item is ComboBox) item = item as ComboBox;
                else return;

                item.BorderBrush =
                    (SolidColorBrush)new BrushConverter()
                    .ConvertFromString("#121212");
            } 
        }

        private void TestDictionaryFilenameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestDictionaryFilenameComboBox.SelectedIndex != 0)
                return;

            var opener = new OpenFileDialog()
            {
                Multiselect = true,
                RestoreDirectory = true,
                Filter = "text files|*.txt;*.lml;*.log",
            };

            if (opener.ShowDialog() == true)
            {
                var files = opener.FileNames;
                foreach (var file in files)
                {
                    if (!TestManager.Data.RecentTestDictionaries.Contains(file))
                    {
                        TestManager.Data.RecentTestDictionaries.Add(file);
                        TestDictionaryFilenameComboBox.Items.Insert(1, file);
                    }
                }
            }

            TestDictionaryFilenameComboBox.SelectedIndex = 1;
        }

        private void RefreshWordsButton_Click(object sender, RoutedEventArgs e)
        {
            var fileText = File.ReadAllText(TestDictionaryFilenameComboBox.SelectedItem.ToString());
            var words = fileText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < words.Count; i++)
                words[i] = words[i].Replace("\r", "");

            SetStandardBorderBrush(
                FirstWordIndexTextBox,
                LastWordIndexTextBox);

            var successfulReading = int.TryParse(FirstWordIndexTextBox.Text, out int firstWordIndex);
            if (!successfulReading || firstWordIndex <= 0)
            {
                InvalidDataHighlighting(FirstWordIndexTextBox);
                return;
            }

            successfulReading = int.TryParse(LastWordIndexTextBox.Text, out int lastWordIndex);
            if (!successfulReading || lastWordIndex <= 0)
            {
                InvalidDataHighlighting(LastWordIndexTextBox);
                return;
            }

            var rangedWords = words.Skip(firstWordIndex - 1).Take(Math.Abs(lastWordIndex - firstWordIndex));
            var text = "";

            int n = 0;
            foreach (var word in rangedWords) text += $"{++n:D4}:\t{word}\n";
            LessonDataTextBox.Text = text;
        }
    }
}
