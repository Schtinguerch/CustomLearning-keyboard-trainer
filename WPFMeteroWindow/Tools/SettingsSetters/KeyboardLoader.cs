using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFMeteroWindow.Properties;
using LmlLibrary;

namespace WPFMeteroWindow
{
    public static class KeyboardLoader
    {
        public static (string[][], Button[]) LoadButtons(this Grid grid, string filename)
        {
            ClearKeys(grid);

            var keyboardData = new Lml(filename, Lml.Open.FromFile);
            var keyData = new string[61][];
            var buttons = new Button[61];
            for (int i = 0; i < 61; i++)
            {
                keyData[i] = keyboardData.GetArray($"Layout>k{i}");

                for (int j = 0; j < 4; j++)
                    keyData[i][j] = keyData[i][j].ToBeCorrected();
            }


            int buttonIndex = -1, keyCount = 14;

            int[] keysInRow = { 14, 14, 13, 12, 8 };
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < keysInRow[i]; j++)
                {
                    buttonIndex++;
                    var button = NewKeyboardButton(keyData[buttonIndex][0], keyData[buttonIndex][1],
                        keyData[buttonIndex][2], keyData[buttonIndex][3]);

                    buttons[buttonIndex] = button;

                    Grid.SetColumn(button, j);
                    
                    var rowGrid = (Grid)grid.Children[i];
                    rowGrid.Children.Add(button);
                }
            }

            return (keyData, buttons);
        }

        public static void SetContent(Button targetButton, string defaultKey, string shiftKey, string altGrKey, string shiftAltGrKey)
        {
            var scale = new ScaleTransform(1.0, 1.0);
            targetButton.RenderTransformOrigin = new Point(0.5, 0.5);
            targetButton.RenderTransform = scale;

            var isModifierKey = (defaultKey == "ctrl") || (defaultKey == "shift") || (defaultKey == "caps") ||
                                (defaultKey == "win") || (defaultKey == "alt") || (defaultKey == "altGr") ||
                                (defaultKey == "enter") || (defaultKey == "back") || (defaultKey == "tab") || (defaultKey == "menu");

            double boxSizeX = 32d, boxSizeY = 32d;
            double multLeft = 0.6, multRight = 0.4, multUp = 0.67, multBottom = 0.33;
            if (isModifierKey)
            {
                boxSizeX = 50d;
                multLeft = 1;
                multRight = 0;
                multUp = 1;
                multBottom = 0;
            }

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.ColumnDefinitions[0].Width = new GridLength(multLeft * boxSizeX, GridUnitType.Pixel);
            grid.ColumnDefinitions[1].Width = new GridLength(multRight * boxSizeX, GridUnitType.Pixel);

            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            grid.RowDefinitions[0].Height = new GridLength(multUp * boxSizeY, GridUnitType.Pixel);
            grid.RowDefinitions[1].Height = new GridLength(multBottom * boxSizeY, GridUnitType.Pixel);

            targetButton.Content = grid;

            var textBlockStyle = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter(TextBlock.ForegroundProperty, new BrushConverter().ConvertFromString(Settings.Default.KeyboardFontColor) as SolidColorBrush),
                    new Setter(TextBlock.FontFamilyProperty, new FontFamily(Settings.Default.KeyboardFont)),
                    new Setter(TextBlock.FontSizeProperty, (double) 22),
                    new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                    new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                }
            };

            var smallTextBlockStyle = textBlockStyle;
            smallTextBlockStyle.Setters[2] = new Setter(TextBlock.FontSizeProperty, (double)12);

            TextBlock defaultTextBlock;
            if (isModifierKey)
            {
                defaultTextBlock = new TextBlock()
                { Text = defaultKey, Style = textBlockStyle };
            }
            else
            {
                defaultTextBlock = new TextBlock()
                { Text = (defaultKey.Length != 2) ? (defaultKey.ToUpper() == shiftKey? defaultKey.ToUpper() : defaultKey) : defaultKey[1].ToString(), Style = textBlockStyle, FontSize = 22d };
            }

            TextBlock
                shiftTextBlock = new TextBlock() { Text = (defaultKey.Length != 2) ? (defaultKey.ToUpper() != shiftKey) ? shiftKey : "" : defaultKey[1].ToString(), Style = smallTextBlockStyle },
                altGrTextBlock = new TextBlock() { Text = (altGrKey.Length != 2) ? altGrKey : altGrKey[1].ToString(), Style = smallTextBlockStyle },
                shiftAltGrTextBlock = new TextBlock() { Text = (shiftAltGrKey.Length != 2) ? shiftAltGrKey : shiftAltGrKey[1].ToString(), Style = smallTextBlockStyle };

            Grid.SetColumn(defaultTextBlock, 0);
            Grid.SetRow(defaultTextBlock, 0);

            Grid.SetColumn(shiftTextBlock, 1);
            Grid.SetRow(shiftTextBlock, 0);

            Grid.SetColumn(altGrTextBlock, 1);
            Grid.SetRow(altGrTextBlock, 1);

            Grid.SetColumn(shiftAltGrTextBlock, 0);
            Grid.SetRow(shiftAltGrTextBlock, 1);

            grid.Children.Add(defaultTextBlock);
            grid.Children.Add(shiftTextBlock);
            grid.Children.Add(altGrTextBlock);
            grid.Children.Add(shiftAltGrTextBlock);
        }

        public static void SetContent(Button targetButton, string[] keys) =>
            SetContent(targetButton, keys[0], keys[1], keys[2], keys[3]);
    

        private static Button NewKeyboardButton(string defaultKey, string shiftKey, string altGrKey, string shiftAltGrKey)
        {
            var padding = 1d;

            var button = new Button()
            {
                Margin = new Thickness(padding),
                Background =
                    new BrushConverter().ConvertFromString(Settings.Default.KeyboardBackgroundColor) as SolidColorBrush,
                BorderBrush =
                    new BrushConverter().ConvertFromString(Settings.Default.KeyboardBorderColor) as SolidColorBrush,
            };

            SetContent(button, defaultKey, shiftKey, altGrKey, shiftAltGrKey);
            return button;
        }
    
        private static void ClearKeys(Grid grid)
        {
            foreach (var element in grid.Children)
            {
                if (element is Grid)
                {
                    var gridRow = element as Grid;
                    gridRow.Children.Clear();
                }
            }
        }
    }
}