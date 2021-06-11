using WPFMeteroWindow.Properties;
using System.Collections.Generic;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    public class ColorSetter : Command
    {
        public override string Name { get; set; } = "cl";

        public override void Run(List<string> arguments, object processingObject = null)
        {
            if (arguments == null) return;
            if (arguments.Count != 2) return;

            var areSettingChanged = true;

            switch (arguments[0])
            {
                case "main":
                    SetColor.FirstColor(UnitedStringArgument);
                    break;

                case "secd":
                    SetColor.SecondColor(UnitedStringArgument);
                    break;

                case "clim":
                    SetColor.CommandLineFirstColor(UnitedStringArgument);
                    break;

                case "clis":
                    SetColor.CommandLineSecondColor(UnitedStringArgument);
                    break;

                case "kbbg":
                    SetColor.KeyboardBackground(UnitedStringArgument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;

                case "kbbr":
                    SetColor.KeyboardBorder(UnitedStringArgument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;

                case "kbhl":
                    SetColor.KeyboardHighlight(UnitedStringArgument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;

                case "kber":
                    SetColor.KeyboardErrorHighlight(UnitedStringArgument);
                    KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
                    break;

                case "t":
                    switch (UnitedStringArgument)
                    {
                        case "lt":
                            SetColor.ColorScheme(Theme.Light);
                            break;

                        case "dt":
                            SetColor.ColorScheme(Theme.Dark);
                            break;

                        default:
                            var color = UnitedStringArgument;
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