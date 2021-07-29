using System;
using System.Windows;
using System.Windows.Media;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public class KeyboardPresenter
    {
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
                            Intermediary.KeyboardData.buttons[i].Background =
                                new BrushConverter().ConvertFromString(Settings.Default.KeyboardBackgroundColor)
                                    as SolidColorBrush;

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

                Intermediary.KeyboardData.buttons[keyIndex].Background =
                    new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor) as SolidColorBrush;
            }
            catch
            {

            }
        }
        
        public static void ShowErrorTyping(char errorCharacter)
        {
            int keyIndex = -1;

            try
            {
                for (int i = 0; i < 61; i++)
                    for (int j = 0; j < 4; j++)
                        if ((Intermediary.KeyboardData.keys[i][j] != "") && (Intermediary.KeyboardData.keys[i][j] != null))
                            if (Intermediary.KeyboardData.keys[i][j][0] == errorCharacter && Intermediary.KeyboardData.keys[i][j].Length < 2)
                                keyIndex = i;
                
                Intermediary.KeyboardData.buttons[keyIndex].Background 
                    = new BrushConverter().ConvertFromString(Settings.Default.KeyboardErrorHighlightColor) as SolidColorBrush;
            }
            
            catch { }
        }

        private static void ShowTextHint(int keyIndex, int statusIndex)
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
                    Intermediary.KeyboardData.buttons[52].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.App.rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uPinky}Shift";
                }
                
                else if (statusIndex == 2)
                {
                    Intermediary.KeyboardData.buttons[57].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.App.rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uThumb}AltGr";
                }
                
                else if (statusIndex == 3)
                {
                    Intermediary.KeyboardData.buttons[52].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.KeyboardData.buttons[57].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.App.rightHandTextBlock.Text = $"+ {Localization.uRight}{Localization.uPinky}Shift + {Localization.uRight}{Localization.uThumb}AltGr";
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
                    Intermediary.KeyboardData.buttons[41].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.App.leftHandTextBlock.Text += $" + {Localization.uLeft}{Localization.uPinky}Shift";
                }
                
                else if (statusIndex == 2)
                {
                    Intermediary.KeyboardData.buttons[57].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.App.leftHandTextBlock.Text += $" + {Localization.uRight}{Localization.uThumb}AltGr";
                }
                
                else if (statusIndex == 3)
                {
                    Intermediary.KeyboardData.buttons[41].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.KeyboardData.buttons[57].Background =
                        new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor)
                            as SolidColorBrush;
                    
                    Intermediary.App.leftHandTextBlock.Text += $" + {Localization.uLeft}{Localization.uPinky}Shift + {Localization.uRight}{Localization.uThumb}AltGr";
                }
            }
        }
    }
}