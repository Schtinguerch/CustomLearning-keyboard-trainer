using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFMeteroWindow
{
    public class GraphDrawer
    {
        private List<SpeedPoint> _speedPoints;

        private double _fieldWidth;

        private double _fieldHeight;

        private double _itemCount;

        public float MaxCPM;
        
        public GraphDrawer(List<SpeedPoint> speedPoints, double fieldHeight, double fieldWidth)
        {
            _speedPoints = speedPoints;
            _itemCount = speedPoints.Count - 1;

            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            var maxCPM = speedPoints[0].CPM;
            foreach (var speedPoint in speedPoints)
                if (speedPoint.CPM > maxCPM)
                    maxCPM = speedPoint.CPM;

            MaxCPM = maxCPM;
        }

        private Point Inverted(SpeedPoint point)
        {
            var invertedPoint = new Point();
            invertedPoint.X = _fieldWidth * point.PartPoint / _itemCount;
            invertedPoint.Y = _fieldHeight * (1 - point.CPM / MaxCPM);

            return invertedPoint;
        }

        public void DrawSpeedGraph(ref Polyline polyline)
        {
            polyline.Points = new PointCollection();
            foreach (var speedPoint in _speedPoints)
            {
                var point = Inverted(speedPoint);
                if (point.Y > _fieldHeight)
                    point.Y = _fieldHeight;
                
                polyline.Points.Add(point);
            }
                
        }
    }
}