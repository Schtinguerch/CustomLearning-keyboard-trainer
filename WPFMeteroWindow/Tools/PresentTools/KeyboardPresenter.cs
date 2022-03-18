using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public class KeyboardPresenter
    {
        //56 - spacebar button
        private int _previousButtonIndex = 56;

        private SolidColorBrush _basicBrush = XamlManager.FindResource<SolidColorBrush>("KeyboardBackgroundColor");
        private SolidColorBrush _highlightBrush = XamlManager.FindResource<SolidColorBrush>("KeyboardHighlightColor");
        private SolidColorBrush _errorHighlightBrush = XamlManager.FindResource<SolidColorBrush>("KeyboardErrorHighlightColor");

        public LightingDonut[] Donuts { get; set; }

        public void ShowTheNecessaryHints(char character)
        {
            int keyIndex = -1, statusIndex = -1;

            try
            {
                for (int i = 0; i < 61; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((Intermediary.KeyboardData.keys[i][j] != "") && (Intermediary.KeyboardData.keys[i][j] != null))
                        {
                            Intermediary.KeyboardData.buttons[i].Background = _basicBrush;
                            var keyCode = Intermediary.KeyboardData.keys[i][j];
                            
                            if (keyCode[0] == character && keyCode.Length < 2 || (keyCode.Length == 2 && keyCode[1] == character))
                            {
                                keyIndex = Intermediary.KeyboardCharIndex = i;
                                statusIndex = Intermediary.KeyboardModifierIndex = j;
                            }
                        }
                    }
                }
                
                ShowTextHint(keyIndex, statusIndex);
                Intermediary.KeyboardData.buttons[keyIndex].Background = _highlightBrush;

                PressButtonAnimation(Intermediary.KeyboardData.buttons[_previousButtonIndex]);
                SplashAnimation(_previousButtonIndex);

                GrowButtonAnimation(Intermediary.KeyboardData.buttons[keyIndex]);
                _previousButtonIndex = keyIndex;
            }
            catch
            {

            }
        }

        private Storyboard _growUpStoryboard = new Storyboard()
        {
            Children = new TimelineCollection()
            {
                new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(60),
                    From = 1,
                    To = 1.12,
                },

                new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(60),
                    From = 1,
                    To = 1.12,
                },
            }
        };

        private Storyboard _slipStoryboard = new Storyboard()
        {
            Children = new TimelineCollection()
            {
                new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(100),
                    From = 1.12,
                    To = 1,
                },

                new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(100),
                    From = 1.12,
                    To = 1,
                },
            }
        };

        private void GrowButtonAnimation(Button button)
        {
            Storyboard.SetTargetProperty(_growUpStoryboard.Children[0], new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(_growUpStoryboard.Children[1], new PropertyPath("RenderTransform.ScaleY"));

            Storyboard.SetTarget(_growUpStoryboard.Children[0], button);
            Storyboard.SetTarget(_growUpStoryboard.Children[1], button);

            _growUpStoryboard.Begin();
        }

        private void PressButtonAnimation(Button button)
        {
            Storyboard.SetTargetProperty(_slipStoryboard.Children[0], new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(_slipStoryboard.Children[1], new PropertyPath("RenderTransform.ScaleY"));

            Storyboard.SetTarget(_slipStoryboard.Children[0], button);
            Storyboard.SetTarget(_slipStoryboard.Children[1], button);

            _slipStoryboard.Begin();
        }

        private void SplashAnimation(int buttonIndex)
        {
            int[] keysInRow = { 14, 14, 13, 12, 8 };

            int rowIndex = 0, columnIndex = 0, countedIndex = -1;

            for (int i = 0; i < keysInRow.Length; i++)
            {
                for (int j = 0; j < keysInRow[i]; j++)
                {
                    countedIndex += 1;
                    if (countedIndex == buttonIndex)
                    {
                        rowIndex = i;
                        columnIndex = j;
                        break;
                    }
                }
            }

            Grid.SetColumn(Donuts[rowIndex], columnIndex);
            Donuts[rowIndex].StartSplash();
        }

        public void ShowErrorTyping(char errorCharacter)
        {
            int keyIndex = -1;

            try
            {
                for (int i = 0; i < 61; i++)
                    for (int j = 0; j < 4; j++)
                        if ((Intermediary.KeyboardData.keys[i][j] != "") && (Intermediary.KeyboardData.keys[i][j] != null))
                            if (Intermediary.KeyboardData.keys[i][j][0] == errorCharacter && Intermediary.KeyboardData.keys[i][j].Length < 2)
                                keyIndex = i;

                Intermediary.KeyboardData.buttons[keyIndex].Background = _errorHighlightBrush;
            }
            
            catch { }
        }

        private void ShowTextHint(int keyIndex, int statusIndex)
        {
            if (keyIndex == 56)
            {
                Intermediary.App.rightHandTextBlock.Text = $"{Localization.uThumb} {Localization.uSpace}";
                Intermediary.App.leftHandTextBlock.Text = "";
            }
            else if (keyIndex.IsInRange(0, 5) || keyIndex.IsInRange(15, 19) || keyIndex.IsInRange(29, 33) ||
                     keyIndex.IsInRange(42, 46))
            {
                Intermediary.App.rightHandTextBlock.Text = "";
                Intermediary.App.leftHandTextBlock.Text = Localization.uLeft;
                if (keyIndex.IsEqualOr(0, 1, 15, 29, 42))
                    Intermediary.App.leftHandTextBlock.Text += Localization.uPinky + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                else if (keyIndex.IsEqualOr(2, 16, 30, 43))
                    Intermediary.App.leftHandTextBlock.Text += Localization.uRing + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                else if (keyIndex.IsEqualOr(3, 17, 31, 44))
                    Intermediary.App.leftHandTextBlock.Text += Localization.uMiddle + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                else
                    Intermediary.App.leftHandTextBlock.Text += Localization.uIndex + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                

                if (statusIndex == 0)
                    Intermediary.App.rightHandTextBlock.Text = "";
                
                else if (statusIndex == 1)
                {
                    Intermediary.KeyboardData.buttons[52].Background = _highlightBrush;
                    Intermediary.App.rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uPinky}Shift";
                }
                
                else if (statusIndex == 2)
                {
                    Intermediary.KeyboardData.buttons[57].Background = _highlightBrush;
                    Intermediary.App.rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uThumb}AltGr";
                }
                
                else if (statusIndex == 3)
                {
                    Intermediary.KeyboardData.buttons[52].Background = _highlightBrush;
                    Intermediary.KeyboardData.buttons[57].Background = _highlightBrush;

                    Intermediary.App.rightHandTextBlock.Text = 
                        $"+ {Localization.uRight}{Localization.uPinky}Shift + {Localization.uRight}{Localization.uThumb}AltGr";
                }
            }
            
            else
            {
                Intermediary.App.leftHandTextBlock.Text = "";
                Intermediary.App.rightHandTextBlock.Text = Localization.uRight;

                if (keyIndex.IsEqualOr(13, 12, 11, 10, 27, 26, 25, 24, 40, 39, 38, 52, 51))
                    Intermediary.App.rightHandTextBlock.Text += Localization.uPinky + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                else if (keyIndex.IsEqualOr(9, 23, 37, 50))
                    Intermediary.App.rightHandTextBlock.Text += Localization.uRing + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                else if (keyIndex.IsEqualOr(8, 22, 36, 49))
                    Intermediary.App.rightHandTextBlock.Text += Localization.uMiddle + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                else
                    Intermediary.App.rightHandTextBlock.Text += Localization.uIndex + Intermediary.KeyboardData.keys[keyIndex][0].ToUpper();
                
                
                if (statusIndex == 0)
                    Intermediary.App.rightHandTextBlock.Text += "";
                
                else if (statusIndex == 1)
                {
                    Intermediary.KeyboardData.buttons[41].Background = _highlightBrush;
                    Intermediary.App.leftHandTextBlock.Text += $" + {Localization.uLeft}{Localization.uPinky}Shift";
                }
                
                else if (statusIndex == 2)
                {
                    Intermediary.KeyboardData.buttons[57].Background = _highlightBrush;
                    Intermediary.App.leftHandTextBlock.Text += $" + {Localization.uRight}{Localization.uThumb}AltGr";
                }
                
                else if (statusIndex == 3)
                {
                    Intermediary.KeyboardData.buttons[41].Background = _highlightBrush;
                    Intermediary.KeyboardData.buttons[57].Background = _highlightBrush;

                    Intermediary.App.leftHandTextBlock.Text += 
                        $" + {Localization.uLeft}{Localization.uPinky}Shift + {Localization.uRight}{Localization.uThumb}AltGr";
                }
            }
        }
    }
}