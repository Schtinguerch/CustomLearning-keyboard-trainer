using System;
using System.Windows;
using System.Windows.Controls;

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
            _editor.LoadWordsViaExplorer();
            WordDataTextBox.Text = _editor.Data;
        }

        private void SaveLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            _editor.NecessaryCPM = Convert.ToInt32(PracticingCpmTextBox.Text);
            _editor.MaxAcceptableMistakes = Convert.ToInt32(PracticingMaxMistakesTextBox.Text);
            _editor.WordCountInLesson = Convert.ToInt32(WordCountInLessonTextBox.Text);
            _editor.RepeatsCount = Convert.ToInt32(WordRepeatsInLessonTextBox.Text);

            _editor.UnloadToCourse();
            Intermediary.CoursePage.DisplayDataFromEditor();
        }
    }
}
