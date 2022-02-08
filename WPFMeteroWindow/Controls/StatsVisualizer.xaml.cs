using System;
using System.Collections.Generic;
using System.Globalization;
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
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Controls
{
    /// <summary>
    /// Логика взаимодействия для StatsVisualizer.xaml
    /// </summary>
    public partial class StatsVisualizer : UserControl
    {
        private List<List<double>> _plotValues;

        private List<string> _argumentSummaries;

        private List<string> _times;

        private List<int> _mistakes;

        private List<string> _mistakeCharacters;

        private double _maxValue = 0d;

        public StatsVisualizer(List<List<double>> plotValues,List<string> times = null, List<string> argumentSummaries = null, List<int> mistakeTimes = null, List<string> mistakeChacacters = null, double averageValue = -1, int scalarCount = -1)
        {
            Loaded += (ss, ee) => 
            {
                try
                {
                    DrawPlots(averageValue);
                    AddValueScalars(scalarCount == -1 ? 10 : scalarCount);
                    AddSummaries();
                }

                catch
                {
                    //I'm just lazy programmer, and I know if there's no data it will throw Exceptions
                }
            };

            if (plotValues == null)
                throw new ArgumentNullException("Plot list cannot be empty");

            InitializeComponent();

            _plotValues = plotValues.CloneList();
            _argumentSummaries = argumentSummaries.CloneList();

            _times = times.CloneList();
            _mistakes = mistakeTimes.CloneList();
            _mistakeCharacters = mistakeChacacters.CloneList();

            foreach (var plot in plotValues)
                foreach (var number in plot)
                    if (number > _maxValue)
                        _maxValue = number;

            _maxValue *= 1.2;
            int maxValueRounded = (int) _maxValue;

            while (maxValueRounded % 10 != 0) maxValueRounded += 1;
            _maxValue = maxValueRounded;

            _plotValues.Reverse();
        }

        private void DrawPlots(double avValue)
        {
            var averageValue = avValue == -1? _plotValues.Last().Average() : avValue;

            AverageSpeedLine.Margin = new Thickness(0, Inverted(new Point(0, averageValue), 10).Y, 0, 0);
            AverageSpeedTextBlock.Text = $"{averageValue:N} {Localization.uCPM}";

            if (_times != null)
            {
                double typingTimeElapsed = (double) _times[_times.Count - 1].TimeToMilliseconds();
                foreach (var mistake in _mistakes)
                {
                    double canvasPosition = PlotCanvas.ActualWidth * mistake / typingTimeElapsed;
                    PlotCanvas.Children.Add(new Rectangle()
                    {
                        Fill = new BrushConverter().ConvertFromString(Settings.Default.KeyboardErrorHighlightColor) as SolidColorBrush,
                        Opacity = 0.16,
                        Margin = new Thickness(canvasPosition, 1, 0, 1),
                        Width = 2,
                        Height = PlotCanvas.ActualHeight - 2,
                    });
                }
            }

            for (int i = 0; i < _plotValues.Count - 1; i++)
                PlotCanvas.Children.Add(SplineFromPoints(
                    values: _plotValues[i],
                    strokeBrush: Settings.Default.KeyboardHighlightColor,
                    opacity: 0.3d,
                    strokeThickness: 2d));

            PlotCanvas.Children.Add(SplineFromPoints(
                values: _plotValues[_plotValues.Count - 1],
                strokeBrush: Settings.Default.KeyboardHighlightColor,
                opacity: 1d,
                strokeThickness: 3d));

            if (_times != null)
            {
                int typingTimeElapsed = _times[_times.Count - 1].TimeToMilliseconds();
                int lastIndex = 0;

                int currentIndex = -1;
                foreach (var mistake in _mistakes)
                {
                    double canvasPosition = PlotCanvas.ActualWidth * (double)mistake / typingTimeElapsed;

                    var mainValueList = _plotValues.Last();
                    var currentPoint = new Point(0, 0);

                    var selectedPoint = currentPoint;
                    double rangeToMouse = double.MaxValue;

                    for (int i = lastIndex; i < mainValueList.Count; i++)
                    {
                        currentPoint = Inverted(new Point(i, mainValueList[i]), mainValueList.Count - 1.5);
                        double currentRange = Math.Abs(currentPoint.X - canvasPosition);

                        if (currentRange < rangeToMouse)
                        {
                            rangeToMouse = currentRange;
                            lastIndex = i;
                            selectedPoint = currentPoint;
                        }

                        else
                            break;
                    }

                    PlotCanvas.Children.Add(new MistakeCross()
                    {
                        Margin = new Thickness(currentPoint.X - 5, currentPoint.Y - 5, 0, 0),
                    });

                    PlotCanvas.Children.Add(new TextBlock()
                    {
                        Text = _mistakeCharacters[++currentIndex],
                        Background = new BrushConverter().ConvertFromString(Settings.Default.SecondBackground) as SolidColorBrush,
                        Margin = new Thickness(selectedPoint.X - 8, selectedPoint.Y - 28, 0, 0),

                        TextAlignment = TextAlignment.Center,
                        Width = 16,
                    }) ;
                }
            }
        }

        private void AddSummaries()
        {
            if (_argumentSummaries == null)
            {
                return;
            }

            ArgumentCanvas.Children.Clear();

            for (int i = 0; i < _argumentSummaries.Count; i++)
            {
                var textBlock = new TextBlock()
                {
                    Text = _argumentSummaries[i],
                    Margin = new Thickness(0),
                };

                double offset = MeasureString(textBlock).Width + 20;
                textBlock.Width = offset;

                if (i == 0)
                {
                    textBlock.TextAlignment = TextAlignment.Left;
                    offset = 0;
                }
                    
                else if (i == _argumentSummaries.Count - 1)
                {
                    textBlock.TextAlignment = TextAlignment.Right;
                }
                    
                else
                {
                    textBlock.TextAlignment = TextAlignment.Center;
                    offset /= 2;
                }

                ArgumentCanvas.Children.Add(textBlock);
                var offsetX = ArgumentCanvas.ActualWidth * i / (_argumentSummaries.Count - 1) - offset;

                Canvas.SetLeft(textBlock, offsetX);
                Canvas.SetTop(textBlock, 2d);
            }
        }

        private void AddValueScalars(int scalarCount = 10)
        {
            ValueCanvas.Children?.Clear();
            for (int i = 0; i <= scalarCount; i++)
            {
                var textBlock = new TextBlock();
                textBlock.Text = $"{(_maxValue * i) / scalarCount:N2}";

                ValueCanvas.Children.Add(textBlock);
                var offsetY = PlotCanvas.ActualHeight * (1 - (double) i / scalarCount) - 8;

                if (i == 0)
                {
                    offsetY = PlotCanvas.ActualHeight * (1 - i / scalarCount) - 16;
                }

                else if (i == scalarCount)
                {
                    offsetY = PlotCanvas.ActualHeight * (1 - i / scalarCount);
                } 

                Canvas.SetTop(textBlock, offsetY);
            }
        }

        private Path SplineFromPoints(List<double> values, string strokeBrush, double opacity, double strokeThickness)
        {
            var points = new List<Point>(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                points.Add(Inverted(new Point(i, values[i]), values.Count - 1.5));
            }

            var path = new Path()
            {
                Stroke = new BrushConverter().ConvertFromString(strokeBrush) as SolidColorBrush,
                StrokeThickness = strokeThickness,
                Opacity = opacity,
            };

            path.Data = new PathGeometry
            (
                new PathFigureCollection()
                {
                    new PathFigure()
                    {
                        StartPoint = points[0],
                        Segments = new PathSegmentCollection()
                        {
                            new PolyBezierSegment(points, true)
                        }
                    }
                }
            );

            return path;
        }

        private Point Inverted(Point point, double maxArgument)
        {
            var invertedPoint = new Point();
            invertedPoint.X = PlotCanvas.ActualWidth * point.X / maxArgument;
            invertedPoint.Y = PlotCanvas.ActualHeight * (1 - point.Y / _maxValue);
            return invertedPoint;
        }

        private Size MeasureString(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    textBlock.FontFamily,
                    textBlock.FontStyle,
                    textBlock.FontWeight,
                    textBlock.FontStretch),

                    textBlock.FontSize,

                Brushes.Black,
                new NumberSubstitution(), TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void ShowStats(Point mousePoint)
        {
            var mainValueList = _plotValues.Last();
            var currentPoint = new Point(0, 0);
            var selectedPoint = currentPoint;

            if (mainValueList.Count == 0)
                return;

            double rangeToMouse = double.MaxValue;
            double currentValue = mainValueList[0];
            int currentIndex = 0;

            for (int i = 0; i < mainValueList.Count; i++)
            {
                currentPoint = Inverted(new Point(i, mainValueList[i]), mainValueList.Count - 1.5);
                double currentRange = Math.Abs(currentPoint.X - mousePoint.X + 3);

                if (currentRange < rangeToMouse)
                {
                    rangeToMouse = currentRange;
                    currentValue = mainValueList[i];
                    currentIndex = i;
                    selectedPoint = currentPoint;
                }

                else
                    break;
            }

            SpeedTextBlock.Text = $"{currentValue.ToString("N")} {Localization.uCPM}";


            TimeTextBlock.Text = (_times == null || _times.Count != mainValueList.Count)? "" : _times[currentIndex];
            CurrentValuePoint.Margin = new Thickness(selectedPoint.X - 5, selectedPoint.Y - 5, 0, 0);
        }

        private void PlotCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(PlotCanvas);

            double offsetX = 12;

            VerticalAxisLine.Margin = new Thickness(position.X - 3, 0, 0, 0);

            if (position.X > PlotCanvas.ActualWidth - SummaryGrid.ActualWidth - 10)
            {
                offsetX -= SummaryGrid.ActualWidth + offsetX * 2;
            }

            SummaryGrid.Margin = new Thickness(position.X + offsetX, position.Y - (SummaryGrid.ActualHeight / 2), 0, 0);
            ShowStats(position);
        }

        private void PlotCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            VerticalAxisLine.Visibility = Visibility.Hidden;
            SummaryGrid.Visibility = Visibility.Hidden;
            CurrentValuePoint.Visibility = Visibility.Hidden;
        }

        private void PlotCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            VerticalAxisLine.Visibility = Visibility.Visible;
            SummaryGrid.Visibility = Visibility.Visible;
            CurrentValuePoint.Visibility = Visibility.Visible;
        }
    }
}
