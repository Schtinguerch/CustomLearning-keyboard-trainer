using System;
using System.Collections.Generic;
using MessageBox = System.Windows.MessageBox;

namespace WPFMeteroWindow
{
    public static class LogManager
    {
        public static List<string> LogData { get; private set; } = new List<string>();
        
        public static void Log(string log)
        {
            LogData.Add($"{DateTime.Now.ToString()}: {log};");
        }

        public static void ShowLogList()
        {
            var message = "";
            foreach (var logItem in LogData)
                message += logItem + '\n';

            MessageBox.Show(message);
        }
    }
}