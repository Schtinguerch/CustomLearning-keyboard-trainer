using System;
using System.Collections.Generic;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    public class SetProperty : Command
    {
        public override string Name { get; set; }
        
        public override void Run(List<string> arguments, object processingObject = null)
        {
            SetAdditional(arguments);
            if (arguments.Count < 2)
            {
                LogManager.Log("set property command -> error: less than 2 arguments (nothing to set)");
                return;
            }

            var commands = new List<Command>()
            {
                new SetMaxTapperingRadius(),
                new SetCircleShowTime(),
            };
            
            var subProcessor = new CommandProcessor(commands);
            subProcessor.Run(SubCommandName, SubCommandArguments);
        }
    }
}