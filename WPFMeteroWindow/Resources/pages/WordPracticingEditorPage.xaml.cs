using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для WordPracticingEditorPage.xaml
    /// </summary>
    public partial class WordPracticingEditorPage : Page
    {
        private WordPracticingCourseEditor _editor;

        public WordPracticingEditorPage()
        {
            InitializeComponent();
            Intermediary.WordPracticingPage = this;

            _editor = new WordPracticingCourseEditor(Intermediary.CoursePage.Editor.FolderPath);
        }

        private void LoadWordsLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            _editor.LoadWords(PathViaExplorer());
            WordDataTextBox.Text = _editor.Data;
        }

        private void SaveLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            int
                necessaryCpm = CheckAndIndicate(PracticingCpmTextBox),
                maxMistakes = CheckAndIndicate(PracticingMaxMistakesTextBox),
                wordCount = CheckAndIndicate(WordCountInLessonTextBox),
                repeatCount = CheckAndIndicate(WordRepeatsInLessonTextBox);

            if ((necessaryCpm < 0) || (maxMistakes < 0)
               || (wordCount <= 0) || (repeatCount <= 0))
            {
                Intermediary.App.ShowMessage($"{Localization.uError}: {Localization.uInvalidDataInput}");
                return;
            }

            _editor.NecessaryCPM = Convert.ToInt32(PracticingCpmTextBox.Text);
            _editor.MaxAcceptableMistakes = Convert.ToInt32(PracticingMaxMistakesTextBox.Text);
            _editor.WordCountInLesson = Convert.ToInt32(WordCountInLessonTextBox.Text);
            _editor.RepeatsCount = Convert.ToInt32(WordRepeatsInLessonTextBox.Text);

            _editor.Data = WordDataTextBox.Text;
            _editor.UnloadToCourse();

            Intermediary.CoursePage.DisplayDataFromEditor();
        }

        private int CheckAndIndicate(TextBox textBox)
        {
            int count;
            var isValidCount = int.TryParse(textBox.Text, out count);

            if (!isValidCount) 
            {
                textBox.BorderBrush = new BrushConverter().ConvertFromString(Settings.Default.KeyboardErrorHighlightColor) as SolidColorBrush;
                return -1;
            }

            else
            {
                textBox.BorderBrush = new BrushConverter().ConvertFromString("#202020") as SolidColorBrush;
                return count;
            }
        }

        private string PathViaExplorer()
        {
            var opener = new OpenFileDialog()
            {
                Multiselect = false,
                RestoreDirectory = true,
                Filter = "LML-files|*.lml",
            };

            if (opener.ShowDialog() == true)
                return opener.FileName;
            else
                return "";
        }

        private void CancelLessonButton_Click(object sender, RoutedEventArgs e)
        {
            Intermediary.CoursePage.CloseLessonEditorPage();
        }
    }
}
