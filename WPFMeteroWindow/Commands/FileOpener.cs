using System;
using System.Collections.Generic;
using ScriptMaker;

namespace WPFMeteroWindow.Commands
{
    public class FileOpener : Command
    {
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

        public override string Name { get; set; } = "op";

        public override void Run(List<string> arguments, object processingObject = null)
        {
            if (arguments == null)
            {
                LogManager.Log("Command execution error -> no arguments");
                return;
            }

            SetAdditional(arguments);
            
            var hasFileName = arguments.Count > 1;
            var fileName = "";

            if (hasFileName)
            {
                fileName = arguments[1];
                for (int i = 2; i < arguments.Count; i++)
                    fileName += ' ' + arguments[i];
            }

            switch (arguments[0].ToLower())
            {
                case "l":
                    if (hasFileName)
                        Opener.NewLesson(fileName);
                    else
                        Opener.NewLessonViaExplorer();
                    break;

                case "tc":
                    if (hasFileName)
                        TestManager.LoadWords(fileName);
                    else
                        TestManager.LoadWordsViaExplorer();
                    break;

                case "t":
                    if (arguments.Count == 4)
                        try
                        {
                            Opener.NewTest(Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]), ToAdditional(arguments[3]));
                        }

                        catch
                        {
                            LogManager.Log("Command execution error -> invalid number cast");
                        }

                    if (arguments.Count == 3)
                        Opener.NewTest(ToTestWors(arguments[1]), ToAdditional(arguments[2]));

                    if (arguments.Count == 2)
                        Opener.NewTest(ToTestWors(arguments[1]), TestAdditional.None);
                    break;

                case "k":
                    if (hasFileName)
                        Opener.NewKeyboardLayout(fileName);
                    else
                        Opener.NewKeyboardLayoutViaExplorer();
                    break;

                case "c":
                    if (hasFileName)
                        Opener.NewCourse(fileName, 0);
                    else
                        Opener.NewCourseViaExplorer();
                    break;

                default:
                    if (arguments[0].ToString().ToLower().Contains("c") && hasFileName)
                        Opener.NewCourse(fileName,
                            Convert.ToInt32(arguments[0].Replace("c", "")) - 1);
                    break;
            }
        }
    }
}