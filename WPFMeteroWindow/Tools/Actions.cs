﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using LmlLibrary;
using WPFMeteroWindow.Properties;
using System.Collections.Generic;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WPFMeteroWindow
{
    public static class Actions
    {
        public static MainWindow TheWindow;

        public static void FindMainWindow()
        {
            foreach (Window window in Application.Current.Windows)
                if (window.GetType() == typeof(MainWindow))
                {
                    TheWindow = window as MainWindow;
                    break;
                }
        }

        public static (string[][], Button[]) LoadButtons(this Grid grid, string filename)
        {
            var keyboardData = new Lml(filename, Lml.Open.FromFile);
            var keyData = new string[61][];
            Button[] buttons = new Button[61];
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

        public static string ToBeCorrected(this string s)
        {
            var val = s;

            val = val.Replace(";nth;", "");
            val = val.Replace(";ap;", "'");
            val = val.Replace(";qt;", "\"");
            val = val.Replace(";opbr;", "{");
            val = val.Replace(";clbr;", "}");
            val = val.Replace(";space;", " ");

            return val;
        }

        public static Button NewKeyboardButton(string defaultKey, string shiftKey, string altGrKey, string shiftAltGrKey)
        {
            var padding = 1d;
            var button = new Button()
            {
                Margin = new Thickness(padding, padding, padding, padding),
                BorderBrush = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
                Background =
                    new BrushConverter().ConvertFromString(Settings.Default.KeyboardBackgroundColor) as SolidColorBrush,
            };

            button.BorderBrush =
                new BrushConverter().ConvertFromString(Settings.Default.KeyboardBorderColor) as SolidColorBrush;

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

            button.Content = grid;

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
            smallTextBlockStyle.Setters[2] = new Setter(TextBlock.FontSizeProperty, (double) 12);

            TextBlock defaultTextBlock;
            if (isModifierKey)
            {
                defaultTextBlock = new TextBlock()
                    { Text = defaultKey.ToUpper(), Style = textBlockStyle };
            }
            else
            {
                defaultTextBlock = new TextBlock()
                    { Text = defaultKey.ToUpper(), Style = textBlockStyle, FontSize = 22d };
            }

            TextBlock
                shiftTextBlock = new TextBlock() { Text = (defaultKey.ToUpper() != shiftKey)? shiftKey : "", Style = smallTextBlockStyle },
                altGrTextBlock = new TextBlock() { Text = altGrKey, Style = smallTextBlockStyle },
                shiftAltGrTextBlock = new TextBlock() { Text = shiftAltGrKey, Style = smallTextBlockStyle };

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

            return button;
        }

        public static bool IsInRange(this int number, int a, int b) => ((number >= a) && (number <= b));

        public static bool IsEqualOr(this int number, params int[] numbers)
        {
            var value = false;

            foreach (var num in numbers)
            {
                if (num == number)
                {
                    value = true;
                    break;
                }
            }

            return value;
        }

        public static bool IsComboKeyDown(KeyEventArgs e, params Key[] keys)
        {
            bool isKeyDown = true;

            foreach (var key in keys)
            {
                if (!(e.KeyboardDevice.IsKeyDown(key)))
                {
                    isKeyDown = false;
                    break;
                }
            }

            return isKeyDown;
        }

        public static Visibility IsVisible(bool visible)
        {
            if (visible)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public static List<string> GetFileList(string[] badArray)
        {
            var rightFileList = new List<string>();

            var listItem = "";
            foreach (var item in badArray)
            {
                if (!item.Contains(".lml"))
                    listItem += item + ' ';
                else
                {
                    rightFileList.Add(listItem + item);
                    listItem = "";
                }
            }

            return rightFileList;
        }
    }
}
