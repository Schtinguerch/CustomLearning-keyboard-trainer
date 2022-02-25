using System;
using System.Collections.Generic;
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

        public List<string> AllStoryboardNames
        {
            get
            {
                var keyList = new List<string>();

                foreach (var key in Resources.Keys)
                    keyList.Add(key.ToString());

                return keyList;
            }
        }

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
                _splashStoryboard?.Stop();
                var newShape = value.Copy<Grid>();

                Settings.Default.ChosenClickSplashName = newShape.Name;
                _splashStoryboard = FindResource(Settings.Default.ChosenMouseSplashStoryboard) as Storyboard;

                if (Settings.Default.GraphicsQuality != "High")
                    newShape.Effect = null;

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
        }
    }
}
