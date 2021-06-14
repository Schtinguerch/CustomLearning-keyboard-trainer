using System.Collections.Generic;
using WPFMeteroWindow.Properties;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    public class FontSetter : Command
    {
        public override string Name { get; set; } = "ft";

        public override void Run(List<string> arguments, object processingObject = null)
        {
            if (arguments == null) return;
            if (arguments.Count != 3) return;
            
            SetAdditional(arguments);

            var fontProperty = arguments[2];
            var areSettingsChanged = true;

            for (int i = 3; i < arguments.Count; i++)
                fontProperty += ' ' + arguments[i];

            switch (arguments[0])
            {
                case "f":
                    switch (arguments[1])
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
                    switch (arguments[1])
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
                    switch (arguments[1])
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