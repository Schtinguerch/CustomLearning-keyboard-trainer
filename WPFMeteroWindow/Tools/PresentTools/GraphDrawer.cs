using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public class GraphDrawer
    {
        private List<SpeedPoint> _speedPoints;

        private Canvas _canvas;

        private TextBlock _cpmTextBlock;

        private double _fieldWidth;

        private double _fieldHeight;

        private double _itemCount;

        private int _maxMs;

        public float MaxCPM;

        
        public GraphDrawer(Canvas canvas, List<SpeedPoint> speedPoints, double fieldHeight, double fieldWidth)
        {
            _canvas = canvas;
            _speedPoints = speedPoints;
            _itemCount = speedPoints.Count - 1;

            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            if (speedPoints[0].CPM <= 0)
                speedPoints.RemoveAt(0);

            var maxCPM = speedPoints[0].CPM;

            foreach (var speedPoint in speedPoints)
            {
                if (speedPoint.CPM > maxCPM)
                    maxCPM = speedPoint.CPM;
            }


            _maxMs = speedPoints[speedPoints.Count - 1].PartPoint;
            MaxCPM = maxCPM;

            _cpmTextBlock = new TextBlock()
            {
                FontSize = 14d,
                FontFamily = new FontFamily(Settings.Default.SummaryFont),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.SecondBackground)),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.SummaryFontColor)),
                Padding = new Thickness(12d, 6d, 12d, 4d),
                Effect = new DropShadowEffect()
                {
                    Color = (Color)ColorConverter.ConvertFromString(Settings.Default.MainBackground),
                    BlurRadius = 20,
                    ShadowDepth = 3d,
                    Opacity = 0.7d,
                },
            };
            _canvas.Children.Add(_cpmTextBlock);
            HideCPM();
        }

        private Point Inverted(SpeedPoint point)
        {
            var invertedPoint = new Point();
            invertedPoint.X = _fieldWidth * point.PartPoint / _maxMs;
            invertedPoint.Y = _fieldHeight * (1 - point.CPM / MaxCPM );

            return invertedPoint;
        }

        private double Range(Point first, Point second) =>
            Math.Sqrt(Math.Pow(second.X - first.X, 2d) + Math.Pow(second.Y - first.Y, 2d));

        private void ShowCPM(Point mousePoint, string text)
        {
            Canvas.SetTop(_cpmTextBlock, mousePoint.Y - 32d);
            Canvas.SetLeft(_cpmTextBlock, mousePoint.X);

            _cpmTextBlock.Text = $"{text} {Localization.uCPM}";

            if (_cpmTextBlock.Visibility == Visibility.Hidden)
                _cpmTextBlock.Visibility = Visibility.Visible;
        }

        private void HideCPM()
        {
            _cpmTextBlock.Visibility = Visibility.Hidden;
        }

        public void DrawSpeedGraph(Polyline polyline, bool showCpmByMouse = false)
        {
            polyline.Points = new PointCollection();
            foreach (var speedPoint in _speedPoints)
            {
                var point = Inverted(speedPoint);
                if (point.Y > _fieldHeight)
                    point.Y = _fieldHeight;
                
                polyline.Points.Add(point);
            }

            if (showCpmByMouse)
            {
                _canvas.MouseMove += (s, e) =>
                {
                    var mousePoint = e.GetPosition(_canvas);

                    int startIndex = Convert.ToInt32(polyline.Points.Count * mousePoint.X / _fieldWidth) - 5;
                    if (startIndex < 0) startIndex = 0;

                    int choosenIndex = 0;

                    int endIndex = startIndex + 10;
                    if (endIndex > polyline.Points.Count) endIndex = polyline.Points.Count;

                    for (int i = startIndex; i < endIndex; i++)
                        if (Range(mousePoint, polyline.Points[i]) < Range(mousePoint, polyline.Points[choosenIndex]))
                            choosenIndex = i;

                    if (Range(mousePoint, polyline.Points[choosenIndex]) < 32d)
                        ShowCPM(mousePoint, _speedPoints[choosenIndex].CPM.ToString("N"));
                    else
                        HideCPM();
                };
            }
                
        }
    }
}