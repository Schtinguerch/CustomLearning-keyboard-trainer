using System.Windows.Controls;

namespace WPFMeteroWindow
{
    public static class Intermediary
    {
        public static MainWindow App { get; set; }
        
        public static DiscordManager RichPresentManager { get; set; }
        
        public static (string[][] keys, Button[] buttons) KeyboardData { get; set; }
        
        public static int KeyboardCharIndex { get; set; }
        
        public static int KeyboardModifierIndex { get; set; }
    }
}