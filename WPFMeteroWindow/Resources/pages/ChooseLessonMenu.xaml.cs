using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
            InitializeComponent();
            LessonStackPanel.Children.Capacity = 5000;

            foreach (var lesson in lessons)
                AddLesson(lesson);
        }

        private void AddLesson(string filename)
        {
            var textBlock = new TextBlock()
            {
                Text = OptimizedGetLessonName(filename),
                Padding = new Thickness(10, 2, 10, 2),

                Foreground = new BrushConverter().ConvertFromString(Settings.Default.KeyboardFontColor) as SolidColorBrush,
                FontSize = 18d,
                FontFamily = new FontFamily(Settings.Default.SummaryFont),
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
