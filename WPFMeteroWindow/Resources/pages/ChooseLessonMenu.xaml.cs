using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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
            var style = XamlManager.FindResource<Style>("SummaryTextStyle");

            InitializeComponent();
            LessonStackPanel.Children.Capacity = 5000;

            if (lessons == null || lessons.Count == 0)
                return;

            foreach (var lesson in lessons)
                AddLesson(lesson, style);
        }

        private SolidColorBrush _notPassedIndicatorBrush = XamlManager.FindResource<SolidColorBrush>("NotPassedIndicatorBrush");
        private SolidColorBrush _passedIndicatorBrush = XamlManager.FindResource<SolidColorBrush>("PassedIndicatorBrush");
        private SolidColorBrush _failedIndicatorBrush = XamlManager.FindResource<SolidColorBrush>("FailedIndicatorBrush");

        private void AddLesson(string filename, Style style)
        {
            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            var indicator = new Ellipse() 
            { 
                Width = 8, 
                Height = 8, 
                Margin = new Thickness(20, 0, 10, 0),
                Fill = _notPassedIndicatorBrush,
            };

            var textBlock = new TextBlock()
            {
                Text = OptimizedGetLessonName(filename),
                Padding = new Thickness(10, 2, 10, 2),
                Width = 400d,
                Style = style,
                FontSize = 18d,
            };

            stackPanel.MouseEnter += (s, e) => 
            {
                stackPanel.Background = 
                new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor) 
                as SolidColorBrush; 
            };

            stackPanel.MouseLeave += (s, e) =>
            {
                stackPanel.Background =
                new BrushConverter().ConvertFromString(Settings.Default.MainBackground)
                as SolidColorBrush;
            };

            textBlock.MouseDown += (s, e) =>
            {
                CourseManager.CurrentLessonIndex = GetIndexOfTextBlock(s as TextBlock);
                textBlock.Background =
                new BrushConverter().ConvertFromString(Settings.Default.MainBackground)
                as SolidColorBrush;
            };

            stackPanel.Children.Add(indicator);
            stackPanel.Children.Add(textBlock);

            LessonStackPanel.Children.Add(stackPanel);
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

        public void RefreshPassingIndicators()
        {
            foreach (var index in StatisticsManager.CourseMarks.FullyPassedLessons)
            {
                var stackPanel = LessonStackPanel.Children[index] as StackPanel;
                var ellipse = stackPanel.Children[0] as Ellipse;

                ellipse.Fill = _passedIndicatorBrush;
            }

            foreach (var index in StatisticsManager.CourseMarks.PartucularlyPassedLessons)
            {
                var stackPanel = LessonStackPanel.Children[index] as StackPanel;
                var ellipse = stackPanel.Children[0] as Ellipse;

                ellipse.Fill = _failedIndicatorBrush;
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                RefreshPassingIndicators();
        }

        private int GetIndexOfTextBlock(TextBlock textBlock)
        {
            int index = -1;

            foreach (var child in LessonStackPanel.Children)
            {
                index++;

                var stackPanel = child as StackPanel;
                var txtBlock = stackPanel.Children[1] as TextBlock;

                if (textBlock.Equals(txtBlock))
                    return index;
            }

            return 0;
        }
    }
}
