using CommandMakerLibrary;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Commands
{
    public class FontSetter : ICommand
    {
        public string Name { get; set; } = "ft";
        
        public object Value { get; set; }
        
        public void Execute(object[] args)
        {
            if (args == null) return;
            if (args.Length != 3) return;

            var fontProperty = args[2].ToString();
            var areSettingsChanged = true;
            
            for (int i = 3; i < args.Length; i++)
                fontProperty += ' ' + args[i].ToString();
            
            switch (args[0].ToString())
            {
                case "f":
                    switch (args[1].ToString())
                    {
                        case "main":
                            SetFont.MainLetters(fontProperty);
                            break;
                        
                        case "summ":
                            SetFont.SummaryLetters(fontProperty);
                            break;
                        
                        case "kbrd":
                            SetFont.Keyboard(fontProperty);
                            break;
                    }
                    break;
                
                case "c":
                    switch (args[1].ToString())
                    {
                        case "main":
                            SetFont.MainLetters_Color(fontProperty);
                            break;
                        
                        case "summ":
                            SetFont.Summary_Color(fontProperty);
                            break;
                        
                        case "done":
                            SetFont.MainRaidedLetters_Color(fontProperty);
                            break;
                        
                        case "kbrd":
                            SetFont.Keyboard_Color(fontProperty);
                            break;
                    }
                    break;
                
                case "s":
                    switch (args[1].ToString())
                    {
                        case "main":
                            SetFont.MainLetters_Size(fontProperty);
                            break;
                    }
                    break;
                
                default:
                    areSettingsChanged = false;
                    break;
            }
            
            if (areSettingsChanged)
                Settings.Default.Save();
        }
    }
}