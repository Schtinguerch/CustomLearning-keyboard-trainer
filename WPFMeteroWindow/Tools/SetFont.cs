using System.Windows.Media;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public static class SetFont
    {
        public static void MainLetters(string fontFamily)
        {
            Settings.Default.LessonLettersFont = fontFamily;
            Settings.Default.Save();
            Actions.TheWindow.inputTextBlock.FontFamily = new FontFamily(fontFamily);
            Actions.TheWindow.inputTextBox.FontFamily = new FontFamily(fontFamily);
        }

        public static void MainLetters_Color(string fontColor)
        {
            Settings.Default.MainFontColor = fontColor;
            Settings.Default.Save();
        }

        public static void MainRaidedLetters_Color(string fontColor)
        {
            Settings.Default.RaidedFontColor = fontColor;
            Settings.Default.Save();
        }

        public static void MainLetters_Size(double fontsize)
        {
            Settings.Default.LessonLettersFontSize = fontsize.ToString();
            Settings.Default.Save();
        }
        
        public static void MainLetters_Size(string fontsize)
        {
            Settings.Default.LessonLettersFontSize = fontsize;
            Settings.Default.Save();
        }

        public static void SummaryLetters(string fontFamily)
        {
            Settings.Default.SummaryFont = fontFamily;
            Settings.Default.Save();
        }

        public static void Summary_Color(string fontColor)
        {
            Settings.Default.SummaryFontColor = fontColor;
            Settings.Default.Save();
        }

        public static void Keyboard(string fontFamily)
        {
            Settings.Default.KeyboardFont = fontFamily;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }

        public static void Keyboard_Color(string fontColor)
        {
            Settings.Default.KeyboardFontColor = fontColor;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }
    }
}