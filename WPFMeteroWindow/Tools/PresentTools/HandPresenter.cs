using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMeteroWindow
{
    public class HandPresenter
    {
        private Frame _leftFrame;

        private Frame _rightFrame;

        private bool IsEqualOr(int number, params int[] comparisons)
        {
            bool result = false;
            foreach (var compared in comparisons)
                if (number == compared)
                {
                    result = true;
                    break;
                }

            return result;
        }

        private bool IsHandLeft(int charIndex) =>
            IsEqualOr(
                charIndex,
                0, 1, 2, 3, 4, 5,
                14, 15, 16, 17, 18, 19,
                28, 29, 30, 31, 32, 33,
                41, 42, 43, 44, 45, 46);

        private bool IsHandRight(int charIndex) =>
            IsEqualOr(
                charIndex,
                7, 8, 9, 10, 11, 12, 13,
                20, 21, 22, 23, 24, 25, 26, 27,
                34, 35, 36, 37, 38, 39, 40,
                47, 48, 49, 50, 51, 52);

        private bool IsSpace(int charIndex) => 
            charIndex == 56;

        private bool IsStandardPosition(int charIndex) =>
            IsEqualOr(charIndex, 29, 30, 31, 32, 35, 36, 37, 38, 39);

        private void ShowLeftShift() {}

        private void ShowRightShift() {}

        private void ShowStandard()
        {
            ShowLeftStandard();
            ShowRightStandard();
        }

        private void ShowLeftFromCharIndex(int charIndex) =>
            _leftFrame.Source = new Uri($"Resources/Hands/LeftHand/Key{charIndex}.xaml", UriKind.Relative);

        private void ShowRightFromCharIndex(int charIndex) =>
            _rightFrame.Source = new Uri($"Resources/Hands/RightHand/Key{charIndex}.xaml", UriKind.Relative);

        private void ShowLeftStandard() =>
            _leftFrame.Source = new Uri(@"Resources/Hands/StandardPositionLeft.xaml", UriKind.Relative);

        private void ShowRightStandard() =>
            _rightFrame.Source = new Uri(@"Resources/Hands/StandardPositionRight.xaml", UriKind.Relative);

        public HandPresenter(Frame leftFrame, Frame rightFrame)
        {
            _leftFrame = leftFrame;
            _rightFrame = rightFrame;
        }

        public void ShowHandsInStandardPosition() =>
            ShowHands(30, 0);

        public void ShowHandsOnBackspace()
        {
            try
            {
                int keyIndex = -1;
                
                for (int i = 0; i < 61; i++)
                    for (int j = 0; j < 4; j++)
                        if ((Intermediary.KeyboardData.keys[i][j] != "") && (Intermediary.KeyboardData.keys[i][j] != null))
                            if (Intermediary.KeyboardData.keys[i][j] == "Backspace")
                                keyIndex = i;

                if (keyIndex != -1)
                    ShowHands(keyIndex, 0);
            }

            catch
            {
                ShowHandsInStandardPosition();
            }
        }

        public void ShowHands(int charIndex, int modifierStatus)
        {
            if (IsSpace(charIndex))
                ShowStandard();

            else
            {
                if (IsHandLeft(charIndex))
                {
                    if (IsStandardPosition(charIndex))
                        ShowLeftStandard();
                    else
                        ShowLeftFromCharIndex(charIndex);
                    

                    if ((modifierStatus == 1) || (modifierStatus == 3))
                        ShowRightShift();
                    else
                        ShowRightStandard();
                }

                else if (IsHandRight(charIndex))
                {
                    if (IsStandardPosition(charIndex))
                        ShowRightStandard();
                    else
                        ShowRightFromCharIndex(charIndex);
                    
                    if ((modifierStatus == 1) || (modifierStatus == 3))
                        ShowLeftShift();
                    else
                        ShowLeftStandard();
                }
            }
        }

    }
}
