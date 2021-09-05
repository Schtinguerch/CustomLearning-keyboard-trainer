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

        private Line _cpmLine;

        private Ellipse _cpmEllipse;

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

            if (speedPoints[0].CPM < 0 || speedPoints[0].CPM >= 1400)
            {
                speedPoints.RemoveAt(0);
                StatisticsManager.TimePoints.RemoveAt(0);
            }
                

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
                Padding = new Thickness(12d, 6d, 12d, 4d)
            };

            _cpmLine = new Line()
            {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.SecondBackground)),
                StrokeThickness = 2d,

                Y1 = -50d,
                Y2 = 170d,

                Opacity = 0.5d,
            };

            _cpmEllipse = new Ellipse()
            {
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.KeyboardHighlightColor)),
                StrokeThickness = 0d,

                Width = 10d,
                Height = 10d,
            };

            _canvas.Children.Add(_cpmLine);
            _canvas.Children.Add(_cpmTextBlock);
            _canvas.Children.Add(_cpmEllipse);

            Canvas.SetTop(_cpmTextBlock, 150d);
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

        private double Max(double first, double second) => 
            (first > second)? first : second;

        private double Min(double first, double second) =>
            (first < second)? first : second;

        private void ShowCPM(double X, double Y, string text)
        {
            Canvas.SetLeft(_cpmTextBlock, X + 20);
            _cpmTextBlock.Text = text;

            _cpmLine.X1 = X + 20;
            _cpmLine.X2 = X + 20;

            Canvas.SetLeft(_cpmEllipse, X + 20 - _cpmEllipse.Width / 2d);
            Canvas.SetTop(_cpmEllipse, Y - _cpmEllipse.Height / 2d);
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
                _canvas.PreviewMouseMove += (s, e) =>
                {
                    var mousePoint = e.GetPosition(_canvas);

                    for (int i = 0; i < polyline.Points.Count; i++)
                    {
                        if (Math.Abs(mousePoint.X - polyline.Points[i].X) < 1 && i < StatisticsManager.TimePoints.Count)
                        {
                            var message =  $"{StatisticsManager.TimePoints[i]}: {_speedPoints[i].CPM:N} {Localization.uCPM}";
                            ShowCPM(polyline.Points[i].X, polyline.Points[i].Y, message);
                            break;
                        }
                    }
                };
            }

            else
            {
                _cpmTextBlock.Visibility = Visibility.Hidden;
                _cpmLine.Visibility = Visibility.Hidden;
                _cpmEllipse.Visibility = Visibility.Hidden;
            }
                
        }
    }
}