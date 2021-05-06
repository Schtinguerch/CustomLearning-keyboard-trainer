using System;
using System.Collections.Generic;
using MessageBox = System.Windows.MessageBox;

namespace WPFMeteroWindow
{
    public static class LogManager
    {
        public static List<string> LogData { get; private set; } = new List<string>();

        private static string lastLog;

        public static string LogText
        {
            get
            {
                var message = "";
                foreach (var logItem in LogData)
                    message += logItem + '\n';

                return message;
            }
        }
        
        public static void Log(string log)
        {
            LogData.Add($"{DateTime.Now.ToString()}: {log};");
            lastLog = log;
        }

        public static void ShowLogListViaMessageBox() =>
            MessageBox.Show(LogText);

        public static void ShowLastLog() => 
            Intermediary.App.ShowMessage(lastLog);

        public static void ShowMessage(string message) =>
            Intermediary.App.ShowMessage(message);
    }
}