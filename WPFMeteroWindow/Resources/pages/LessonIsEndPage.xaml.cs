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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для LessonIsEndPage.xaml
    /// </summary>
    public partial class LessonIsEndPage : Page
    {
        public LessonIsEndPage()
        {
            InitializeComponent();
            Focus();
            
            var lessonName = Settings.Default.LessonName;
            lessonName = (lessonName == "...") ? "" : $"\"{lessonName}\"";
            EndedLessonHeaderTextBlock.Text = "Урок " + lessonName + " завершён!";

            var typingSpeed = Settings.Default.TypingLength / ((float) Settings.Default.TypingTime / 10) * 60f;
            WPMtextBlock.Text = $"{typingSpeed.ToString("N")} Симв./мин.";
            WPStextBlock.Text = $"{(typingSpeed / 60f).ToString("N")} Симв./сек.";
            ErrorsTextBlock.Text = 
                $"{Settings.Default.TypingErrors} ошибок: {((float)Settings.Default.TypingErrors / Settings.Default.TypingLength * 100).ToString("N")}%";
            TypingTimeTextBlock.Text =
                $"Время: {Settings.Default.TypingTime / 600}:{(Settings.Default.TypingTime / 10) % 60} = {Settings.Default.TypingTime / 10} сек.";
            CharactersCountTextBlock.Text = $"Кол-во симв: {Settings.Default.TypingLength}";
            
            var drawer = new GraphDrawer(Actions.TheWindow.ChartPoints, 180, 380);
            drawer.DrawSpeedGraph(ref TypingSpeedPolyline);
            
            var maxCPM = drawer.MaxCPM;
            MaxCPMtextBlock.Text = maxCPM.ToString();
            
            drawer = new GraphDrawer(Actions.TheWindow.AveragePoints, 180, 380);
            drawer.MaxCPM = maxCPM;
            drawer.DrawSpeedGraph(ref AverageTypingSpeedPolyline);

            var averageCPM = typingSpeed / 60f;
            var PunctierPoints = new List<SpeedPoint>
            {
                new SpeedPoint(0, averageCPM),
                new SpeedPoint(1, averageCPM),
                new SpeedPoint(2, averageCPM),
                new SpeedPoint(3, averageCPM),
                new SpeedPoint(4, averageCPM),
                new SpeedPoint(5, averageCPM),
                new SpeedPoint(6, averageCPM),
                new SpeedPoint(7, averageCPM),
                new SpeedPoint(8, averageCPM),
                new SpeedPoint(9, averageCPM),
                new SpeedPoint(10, averageCPM),
                new SpeedPoint(11, averageCPM),
                new SpeedPoint(12, averageCPM),
                new SpeedPoint(13, averageCPM),
                new SpeedPoint(14, averageCPM),
                new SpeedPoint(15, averageCPM),
                new SpeedPoint(16, averageCPM),
                new SpeedPoint(17, averageCPM),
                new SpeedPoint(18, averageCPM),
            };
            
            drawer = new GraphDrawer(PunctierPoints, 180, 380);
            drawer.MaxCPM = maxCPM;
            drawer.DrawSpeedGraph(ref AverapeCPMpunctierPolyline);
            
            Canvas.SetTop(AverapeCPMtextBlock, AverapeCPMpunctierPolyline.Points[0].Y - 8d);
            AverapeCPMtextBlock.Text = averageCPM.ToString("N");

            try
            {
                PrevLessonButton.Visibility = (Settings.Default.CourseLessonNumber == 0) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
                NextLessonButton.Visibility = NextLessonButton.Visibility = (Settings.Default.CourseLessonNumber == Actions.TheWindow.LessonsCount - 1) || !Settings.Default.IsCourseOpened ? Visibility.Hidden : Visibility.Visible;
            }
            catch
            {
                
            }
        }

        private void PrevLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            Actions.TheWindow.HideSettingGrid();
            Actions.TheWindow.StartPreviouslesson();
        }

        private void CurrLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            Actions.TheWindow.HideSettingGrid();
            Actions.TheWindow.RestartLesson();
        }

        private void NextLessonButton_OnClick(object sender, RoutedEventArgs e)
        {
            Actions.TheWindow.HideSettingGrid();
            Actions.TheWindow.StartNextLesson();
        }
    }
}
