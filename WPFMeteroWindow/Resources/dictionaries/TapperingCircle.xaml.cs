using System;
using System.Windows;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.dictionaries
{
    /// <summary>
    /// Логика взаимодействия для TapperingCircle.xaml
    /// </summary>
    public partial class TapperingCircle : UserControl
    {
        public double TapperingAuraMaxSize { get; set; } = Settings.Default.MaxTapperingAuraSize;

        public double TapperingCirleSize { get; set; } = Settings.Default.MinTapperingAuraSize;
        
        public TapperingCircle()
        {
            InitializeComponent();
            TapperingDuration = Settings.Default.TapperingMilliseconds;
        }

        public double TapperingDuration
        {
            set
            {
                var showTimeSpan = TimeSpan.FromMilliseconds(value);
                var showDuration = new Duration(showTimeSpan);

                TapperingHeightAnimation.Duration = TapperingWidthAnimation.Duration = showDuration;
                TapperingCircleHideAnimation.BeginTime = showTimeSpan;

                var opacityDuration = new Duration(TimeSpan.FromMilliseconds(value / 6d));
                TapperingCircleShowAnimation.Duration = TapperingCircleHideAnimation.Duration = opacityDuration;
            }
        } 
    }
}
