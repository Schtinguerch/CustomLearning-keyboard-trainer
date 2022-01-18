using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ChooseLessonMenu.xaml
    /// </summary>
    public partial class ChooseLessonMenu : UserControl
    {
        public ChooseLessonMenu(List<string> lessons)
        {
            var dictionary = (ResourceDictionary)Application.LoadComponent(new Uri("\\Resources\\dictionaries\\Styles.xaml", UriKind.Relative));
            var style = (Style)dictionary["SummaryTextStyle"];

            InitializeComponent();
            LessonStackPanel.Children.Capacity = 5000;

            foreach (var lesson in lessons)
                AddLesson(lesson, style);
        }

        private void AddLesson(string filename, Style style)
        {
            var textBlock = new TextBlock()
            {
                Text = OptimizedGetLessonName(filename),
                Padding = new Thickness(10, 2, 10, 2),
                Style = style,
                FontSize = 18d,
            };

            textBlock.MouseEnter += (s, e) => 
            { 
                textBlock.Background = 
                new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor) 
                as SolidColorBrush; 
            };

            textBlock.MouseLeave += (s, e) =>
            {
                textBlock.Background =
                new BrushConverter().ConvertFromString(Settings.Default.MainBackground)
                as SolidColorBrush;
            };

            textBlock.MouseDown += (s, e) =>
            {
                CourseManager.CurrentLessonIndex = LessonStackPanel.Children.IndexOf(s as TextBlock);
                textBlock.Background =
                new BrushConverter().ConvertFromString(Settings.Default.MainBackground)
                as SolidColorBrush;
            };

            LessonStackPanel.Children.Add(textBlock);
        }

        private string OptimizedGetLessonName(string filename)
        {
            var data = File.ReadAllText(filename);
            var openTag = "<Name ";

            int startIndex = data.IndexOf(openTag) + 6;
            int length = data.IndexOf(">>") - startIndex;

            if (startIndex == -1)
                return "...";

            return data.Substring(startIndex, length);
        }
    }
}
