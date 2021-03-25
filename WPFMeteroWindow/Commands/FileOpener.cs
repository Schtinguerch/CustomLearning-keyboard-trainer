using System;
using CommandMakerLibrary;

namespace WPFMeteroWindow.Commands
{
    public class FileOpener : ICommand
    {
        public string Name { get; set; } = "op";
        
        public object Value { get; set; }

        private TestAdditional ToAdditional(object token)
        {
            var additional = TestAdditional.None;
            
            switch (token.ToString().ToLower())
            {
                case "dflt":
                    additional = TestAdditional.None;
                    break;
                    
                case "nmbr":
                    additional = TestAdditional.Numbers;
                    break;
                
                case "ttle":
                    additional = TestAdditional.Uppercase;
                    break;
                
                case "pnct":
                    additional = TestAdditional.Punctuation;
                    break;
                
                case "all":
                    additional = TestAdditional.AllAdditionals;
                    break;
            }

            return additional;
        }

        private TestWords ToTestWors(object token)
        {
            var testWords = TestWords.Top100;

            switch (token.ToString().ToLower())
            {
                case "t100":
                    testWords = TestWords.Top100;
                    break;
                
                case "t200":
                    testWords = TestWords.Top200;
                    break;
                
                case "t500":
                    testWords = TestWords.Top500;
                    break;
                
                case "t1000":
                case "tk":
                    testWords = TestWords.Top1000;
                    break;
                
                case "t2000":
                case "t2k":
                    testWords = TestWords.Top2000;
                    break;
                
                case "t5000":
                case "t5k":
                    testWords = TestWords.Top5000;
                    break;
            }

            return testWords;
        }
        
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
                
                case "t":
                    if (args.Length == 4)
                        Opener.NewTest(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), ToAdditional(args[3]));
                    
                    if (args.Length == 3)
                        Opener.NewTest(ToTestWors(args[1]), ToAdditional(args[2]));
                    
                    if (args.Length == 2)
                        Opener.NewTest(ToTestWors(args[1]), TestAdditional.None);
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