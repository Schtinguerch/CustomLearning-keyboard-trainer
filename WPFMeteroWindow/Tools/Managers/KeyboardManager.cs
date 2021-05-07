using System.Windows.Controls;

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


        public static void LoadKeyboardData(string filename)
        {
            Intermediary.KeyboardData = KeyboardLoader.LoadButtons(Board, filename);
            try { if (LessonManager.LeftRoad != null) ShowTypingHint(LessonManager.LeftRoad[0]); } catch {}
        }
            

        public static void ShowTypingHint(char character)
        {
            KeyboardPresenter.ShowTheNecessaryHints(character);
          //  HandPresenter.ShowHands(Intermediary.KeyboardCharIndex, Intermediary.KeyboardModifierIndex);
        }

        public static void ShowTypingError(char character)
        {
            KeyboardPresenter.ShowErrorTyping(character);
          //  HandPresenter.ShowHandsOnBackspace();
        }
    }
}