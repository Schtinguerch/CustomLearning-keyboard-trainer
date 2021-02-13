using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public static class SetColor
    {
        public static void FirstColor(string color)
        {
            Settings.Default.MainBackground = color;
            Settings.Default.Save();
        }

        public static void SecondColor(string color)
        {
            Settings.Default.SecondBackground = color;
            Settings.Default.Save();
        }

        public static void CommandLineFirstColor(string color)
        {
            Settings.Default.ThirdBackground = color;
            Settings.Default.Save();
        }

        public static void CommandLineSecondColor(string color)
        {
            Settings.Default.ThirdSecBackground = color;
            Settings.Default.Save();
        }

        public static void KeyboardBackground(string color)
        {
            Settings.Default.KeyboardBackgroundColor = color;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }

        public static void KeyboardBorder(string color)
        {
            Settings.Default.KeyboardBorderColor = color;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }

        public static void KeyboardHighlight(string color)
        {
            Settings.Default.KeyboardHighlightColor = color;
            Settings.Default.Save();
            Actions.TheWindow.ReloadKeyboard();
        }
    }
}