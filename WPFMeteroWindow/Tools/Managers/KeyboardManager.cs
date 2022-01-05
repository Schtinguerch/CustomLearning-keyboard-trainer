using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public static class KeyboardManager
    {
        public static KeyboardPresenter KeyboardPresenter { get; set; }
        
        public static HandPresenter HandPresenter { get; set; }
        
        public static Grid Board { get; set; }

        public static void LoadKeyboardData(this Grid grid, string filename)
        {
            Intermediary.KeyboardData = KeyboardLoader.LoadButtons(grid, filename);
            try { ShowTypingHint(LessonManager.LeftRoad[0]); } catch {}
        }

        public static void LoadKeyboardData(string filename, bool isFiction = false)
        {
            var recentLayouts = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Settings.Default.RecentLayoutsPath));
            var hasTheSame = false;

            foreach (var layout in recentLayouts)
                if (layout == filename)
                {
                    hasTheSame = true;
                    break;
                }

            if (!hasTheSame)
            {
                recentLayouts.Add(filename);
                File.WriteAllText(
                    Settings.Default.RecentLayoutsPath,
                    JsonConvert.SerializeObject(recentLayouts, Formatting.Indented));
            }

            if (isFiction)
                return;

            if (Settings.Default.ShowHands)
            {
                Intermediary.App.LeftHandFrame.Visibility = Visibility.Visible;
                Intermediary.App.RightHandFrame.Visibility = Visibility.Visible;
            }
            else
            {
                Intermediary.App.LeftHandFrame.Visibility = Visibility.Hidden;
                Intermediary.App.RightHandFrame.Visibility = Visibility.Hidden;
            }

            Intermediary.KeyboardData = KeyboardLoader.LoadButtons(Board, filename);
            try { if (LessonManager.LeftRoad != null) ShowTypingHint(LessonManager.LeftRoad[0]); } catch {}
        }
            

        public static void ShowTypingHint(char character)
        {
            KeyboardPresenter.ShowTheNecessaryHints(character);
            HandPresenter.ShowHands(Intermediary.KeyboardCharIndex, Intermediary.KeyboardModifierIndex);
            //HandPresenter.ShowHands(51, 0);
        }

        public static void ShowTypingError(char character)
        {
            KeyboardPresenter.ShowErrorTyping(character);
           // HandPresenter.ShowHandsOnBackspace();
        }
    }
}