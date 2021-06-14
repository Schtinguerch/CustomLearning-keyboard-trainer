using System;
using System.Collections.Generic;
using ScriptMaker;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Commands
{
    public class SetMaxTapperingRadius : Command
    {
        public override string Name { get; set; } = "auraRadius";

        public override void Run(List<string> arguments, object processingObject = null) =>
            Settings.Default.MaxTapperingAuraSize = Convert.ToDouble(arguments[0]) * 2;
    }
}