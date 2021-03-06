using System.Windows.Media;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public static class SetFont
    {
        public static void MainLetters(string fontFamily)
        {
            Settings.Default.LessonLettersFont = fontFamily;
            Actions.TheWindow.inputTextBlock.FontFamily = new FontFamily(fontFamily);
            Actions.TheWindow.inputTextBox.FontFamily = new FontFamily(fontFamily);
        }

        public static void MainLetters_Color(string fontColor)
        {
            Settings.Default.MainFontColor = fontColor;
        }

        public static void MainRaidedLetters_Color(string fontColor)
        {
            Settings.Default.RaidedFontColor = fontColor;
        }

        public static void MainLetters_Size(double fontsize)
        {
            Settings.Default.LessonLettersFontSize = fontsize.ToString();
        }
        
        public static void MainLetters_Size(string fontsize)
        {
            Settings.Default.LessonLettersFontSize = fontsize;
        }

        public static void SummaryLetters(string fontFamily)
        {
            Settings.Default.SummaryFont = fontFamily;
        }

        public static void Summary_Color(string fontColor)
        {
            Settings.Default.SummaryFontColor = fontColor;
        }

        public static void Keyboard(string fontFamily)
        {
            Settings.Default.KeyboardFont = fontFamily;
            Actions.TheWindow.ReloadKeyboard();
        }

        public static void Keyboard_Color(string fontColor)
        {
            Settings.Default.KeyboardFontColor = fontColor;
            Actions.TheWindow.ReloadKeyboard();
        }
    }
}