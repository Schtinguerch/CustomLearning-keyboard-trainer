using System.Collections.Generic;
using System.Text;
using System.Windows;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    public class PreDefine : Command
    {
        public override string Name { get; set; } = "define";
        
        public override void Run(List<string> arguments, object processingObject = null)
        {
            SetAdditional(arguments);

            var pattern = arguments[0];
            var replacement = SubCommandArguments.ConcatToString();
            var processingCode = (processingObject as StringBuilder).ToString();

            processingCode = processingCode.RemoveFirstLines(1);
            processingCode = processingCode.Replace(pattern, replacement);

            (processingObject as StringBuilder).Clear();
            (processingObject as StringBuilder).Append(processingCode);
            
            Clipboard.SetText(processingCode);
        }
    }
}