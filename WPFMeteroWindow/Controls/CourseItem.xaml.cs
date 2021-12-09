using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для CourseItem.xaml
    /// </summary>
    public partial class CourseItem : UserControl
    {
        private string _courseFullPath;

        public CourseItem()
        {
            InitializeComponent();
        }

        public string CourseName
        {
            get => _courseNameTextBlock.Text;
            set => _courseNameTextBlock.Text = value;
        }

        public int LessonCount
        {
            get => int.Parse(_lessonCountTextBlock.Text.Split(':')[1]);
            set => _lessonCountTextBlock.Text = $"{Localization.uLessonCount}: {value}";
        }

        public string CourseType
        {
            get => _courseTypeTextBlock.Text;
            set => _courseTypeTextBlock.Text = value;
        }

        public string CoursePath
        {
            get => _courseFullPath;
            set
            {
                _courseFullPath = value;

                if (_courseFullPath.Length >= 25)
                {
                    int length = 25;
                    int startIndex = _courseFullPath.Length - length;

                    _coursePathTextBlock.Text = "..." + _courseFullPath.Substring(startIndex, length);
                }
                else
                {
                    _coursePathTextBlock.Text = _courseFullPath;
                }
            }
        }

        private void ItemBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.ThirdBackground);
            ItemBorder.CornerRadius = new CornerRadius(20);
        }
            

        private void ItemBorder_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.MainBackground);
            ItemBorder.CornerRadius = new CornerRadius(16);
            Opener.NewCourse(_courseFullPath, 0);
        }
            
        private void ItemBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.SecondBackground);
            ItemBorder.CornerRadius = new CornerRadius(16);
        }
           

        private void ItemBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ItemBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Settings.Default.MainBackground);
            ItemBorder.CornerRadius = new CornerRadius(10);
        }
            
    }
}
