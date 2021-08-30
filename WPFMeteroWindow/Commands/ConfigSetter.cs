using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    class ConfigSetter : Command
    {
        public override string Name { get; set; } = "cfg";

        public override void Run(List<string> arguments, object processingObject = null)
        {
            if (arguments == null) return;
            if (arguments.Count < 1) return;

            SetAdditional(arguments);

            switch (arguments[0])
            {
                case "c":
                    UserConfigManager.CopyConfigToClipboard();
                    break;

                case "p":
                    UserConfigManager.ImportConfigFromClipboard();
                    break;

                case "imp":
                    if (arguments.Count > 1)
                    {
                        var filename = UnitedStringArgument.Replace("exp ", "");
                        UserConfigManager.ImportConfigFromFile(filename);
                    }
                    else
                        UserConfigManager.ImportConfigViaExplorer();
                    break;

                case "exp":
                    if (arguments.Count > 1)
                    {
                        var filename = UnitedStringArgument.Replace("exp ", "");
                        UserConfigManager.ExportConfigToFile(filename);
                    }
                    else
                        UserConfigManager.ExportConfigViaExplorer();
                    break;
            }
        }
    }
}
