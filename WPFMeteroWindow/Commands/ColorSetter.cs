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
            var areSettingChanged = true;
            var argument = args.ArgsToString(1);
             
            switch (args[0].ToString())
            {
                case "main":
                    SetColor.FirstColor(argument);
                    break;
                
                case "secd":
                    SetColor.SecondColor(argument);
                    break;
                
                case "clim":
                    SetColor.CommandLineFirstColor(argument);
                    break;
                
                case "clis":
                    SetColor.CommandLineSecondColor(argument);
                    break;
                
                case "kbbg":
                    SetColor.KeyboardBackground(argument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;
                
                case "kbbr":
                    SetColor.KeyboardBorder(argument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;
                
                case "kbhl":
                    SetColor.KeyboardHighlight(argument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;

                case "kber":
                    SetColor.KeyboardErrorHighlight(argument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;

                case "t":
                    switch (argument)
                    {
                        case "lt":
                            SetColor.ColorScheme(Theme.Light);
                            break;
                
                        case "dt":
                            SetColor.ColorScheme(Theme.Dark);
                            break;
                        
                        default:
                            var color = argument;
                            SetColor.WindowColor(color);
                            break;
                    }
                    break;
                
                default:
                    areSettingChanged = false;
                    break;
            }
            
            if (areSettingChanged)
                Settings.Default.Save();
        }
    }
}