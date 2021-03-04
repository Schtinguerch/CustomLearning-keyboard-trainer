using System;
using System.Windows;
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
            var appDictionaries = Application.Current.Resources.MergedDictionaries;
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
                
                case "t":
                    switch (args[1].ToString())
                    {
                        case "lt":
                            SetColor.ColorScheme(Theme.Light);
                            break;
                
                        case "dt":
                            SetColor.ColorScheme(Theme.Dark);
                            break;
                        
                        default:
                            var color = args[1].ToString();
                            SetColor.WindowColor(color);
                            break;
                    }
                    break;
            }
            
            Settings.Default.Save();
        }
    }
}