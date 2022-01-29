using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptMaker;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Commands
{
    public class FlagSetter : Command
    {
        public override string Name { get; set; } = "set";

        public override void Run(List<string> arguments, object processingObject = null)
        {
            if (arguments == null) return;
            if (arguments.Count < 2) return;

            switch (arguments[0].ToLower())
            {
                case "ti":
                    if (arguments.Count == 1)
                        Opener.NewTextInputWay("SingleLineWithStaticCaret");

                    switch (arguments[1].ToLower())
                    {
                        case "st":
                        case "sc":
                            Opener.NewTextInputWay("SingleLineWithStaticCaret");
                            break;

                        case "cl":
                        case "c":
                            Opener.NewTextInputWay("Classic");
                            break;

                        case "m":
                        case "co":
                            Opener.NewTextInputWay("Competitive");
                            break;
                    }
                    break;

                case "kl":
                    if (arguments[1] == "1")
                    {
                        Settings.Default.CurrentLayout = 1;
                        Opener.NewKeyboardLayout(Settings.Default.KeyboardLayoutFile);
                    }
                    else
                    {
                        Settings.Default.CurrentLayout = 2;
                        Opener.NewKeyboardLayout(Settings.Default.SecondKeyboardLayoutFile);
                    }
                    break;

                case "tw":
                    int wordCount;
                    var status = int.TryParse(arguments[1], out wordCount);

                    if (status)
                        Settings.Default.TestWordCount = wordCount;
                    break;
            }
        }
    }
}
