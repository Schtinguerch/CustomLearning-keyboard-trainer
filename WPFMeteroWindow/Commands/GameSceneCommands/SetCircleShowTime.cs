using System;
using System.Collections.Generic;
using WPFMeteroWindow.Properties;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    public class SetCircleShowTime : Command
    {
        public override string Name { get; set; } = "showTime";

        public override void Run(List<string> arguments, object processingObject = null) =>
            Settings.Default.TapperingMilliseconds = Convert.ToDouble(arguments[0]);
    }
}