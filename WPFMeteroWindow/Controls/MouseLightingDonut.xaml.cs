using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для MouseLightingDonut.xaml
    /// </summary>
    public partial class MouseLightingDonut : UserControl
    {
        private Storyboard _splashStoryboard;
        public void StartSplash()
        {
            if (!Settings.Default.EnableSplashAnimation)
                return;

            _splashStoryboard.Begin();
        }

        public Grid Shape
        {
            get => ShapeGrid;
            set
            {
                var newShape = value.Copy<Grid>();
                Settings.Default.ChosenClickSplashName = newShape.Name;

                MainGrid.Children.Clear();
                MainGrid.Children.Add(newShape);
            }
        }

        public bool RepeatForever
        {
            get => _splashStoryboard.RepeatBehavior == RepeatBehavior.Forever;
            set
            {
                if (value)
                {
                    _splashStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                    _splashStoryboard.Begin();
                }

                else
                {
                    _splashStoryboard.RepeatBehavior = new RepeatBehavior(1.0);
                    _splashStoryboard.Stop();
                }
            }
        }

        public MouseLightingDonut()
        {
            InitializeComponent();
            _splashStoryboard = FindResource("SplashStoryboard") as Storyboard;
        }
    }
}
