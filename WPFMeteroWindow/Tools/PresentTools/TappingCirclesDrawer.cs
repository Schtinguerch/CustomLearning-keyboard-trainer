using System.Windows;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;
using WPFMeteroWindow.Resources.dictionaries;

namespace WPFMeteroWindow
{
    public class TappingCirclesDrawer
    {
        private (string[][] keys, Button[] buttons) _keyboard;

        private Canvas _canvas;

        public TappingCirclesDrawer(Canvas canvas, (string[][] keys, Button[] buttons) keyboard)
        {
            _canvas = canvas;
            _keyboard = keyboard;
        }

        public void DrawCicle(string character)
        {
            character = character.Substring(0, 1);

            int buttonIndex = 56;
            for (int i = 0; i < _keyboard.keys.Length; i++)
                for (int j = 0; j < 4; j++)
                    if (character == _keyboard.keys[i][j])
                    {
                        buttonIndex = i;
                        break;
                    }

            var relativePoint = _keyboard.buttons[buttonIndex].TranslatePoint(new Point(0, 0), _canvas);
            relativePoint.X -= 200 - _keyboard.buttons[buttonIndex].ActualWidth / 2;
            relativePoint.Y -= 200 - _keyboard.buttons[buttonIndex].ActualHeight / 2;

            var newCircle = new TapperingCircle();

            Canvas.SetLeft(newCircle, relativePoint.X);
            Canvas.SetTop(newCircle, relativePoint.Y);

            _canvas.Children.Add(newCircle);
        }
    }
}