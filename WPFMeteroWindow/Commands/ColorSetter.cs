using WPFMeteroWindow.Properties;
using CommandMakerLibrary;

namespace WPFMeteroWindow.Commands
{
    public class ColorSetter : ICommand
    {
        public string Name { get; set; } = "cl";
        
        public object Value { get; set; }
        
        public void Execute(object[] args)
        {
            switch (args[0].ToString())
            {
                case "main":
                    SetColor.FirstColor(args[1].ToString());
                    break;
                
                case "secd":
                    SetColor.SecondColor(args[1].ToString());
                    break;
                
                case "clim":
                    SetColor.CommandLineFirstColor(args[1].ToString());
                    break;
                
                case "clis":
                    SetColor.CommandLineSecondColor(args[1].ToString());
                    break;
                
                case "kbbg":
                    SetColor.KeyboardBackground(args[1].ToString());
                    break;
                
                case "kbbr":
                    SetColor.KeyboardBorder(args[1].ToString());
                    break;
                
                case "kbhl":
                    SetColor.KeyboardHighlight(args[1].ToString());
                    break;
            }
        }
    }
}