using System;
using CommandMakerLibrary;

namespace WPFMeteroWindow.Commands
{
    public class FileOpener : ICommand
    {
        public string Name { get; set; } = "op";
        
        public object Value { get; set; }
        
        public void Execute(object[] args)
        {
            var fileName = args[1].ToString();
            for (int i = 2; i < args.Length; i++)
                fileName += ' ' + args[i].ToString();

            switch (args[0].ToString().ToLower())
            {
                case "l":
                    Opener.NewLesson(fileName);
                    break;

                case "k":
                    Opener.NewKeyboardLayout(fileName);
                    break;
                
                case "c":
                    Opener.NewCourse(fileName, 0);
                    break;
                
                default:
                    if (args[0].ToString().ToLower().Contains("c"))
                        Opener.NewCourse(fileName, 
                            Convert.ToInt32(args[0].ToString().Replace("c", "")) - 1);
                    break;
            }
        }
    }
}