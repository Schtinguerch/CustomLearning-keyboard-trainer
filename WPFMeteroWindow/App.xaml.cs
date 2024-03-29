﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            SetGlobalTryCatch();

            LanguageManager.SetLanguage(Settings.Default.ChosenLanguageIndex);
            CheckRelativePaths();
        }

        private void SetGlobalTryCatch()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            DispatcherUnhandledException += (s, e) => { };
            TaskScheduler.UnobservedTaskException += (s, e) => { };
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppManager.SaveJsonDataFromManagers();

            var exception = e.ExceptionObject as Exception;
            var message = exception.Message;

            LogManager.Log($"Critical error: {message}");
            File.WriteAllText("logs.log", $"{message}\n\n{LogManager.LogText}");

            MessageBox.Show($"{message}", $"{Localization.uError}");
        }

        private void CheckRelativePaths()
        {
            if (Settings.Default.FirstLaunchFileIndicatorPath.Contains(":\\"))
                return;

            var appFolder = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("\\CustomLearning.exe", "");
            MessageBox.Show(appFolder);

            var fileProperties = new string[]
            {
                "LoadedLessonFile",
                "KeyboardLayoutFile",
                "BackgroundImagePath",
                "TestWordListPath",
                "TapClickSoundFile",
                "ErrorClickSoundFile",
                "SecondKeyboardLayoutFile",

                "RecentCourcesPath",
                "RecentLayoutsPath",
                "RecentConfigs",
                "AllTypingSpeedPath",
                "CourcesStatisticsPath",
                "RecentTestDictionariesPath",

                "ReplaceDictionaryPath",
                "FirstLaunchFileIndicatorPath",
                "RecentPhotosPath",
                "MouseClickSoundFile",
            };

            foreach (var property in fileProperties)
                if (!(Settings.Default[property] as string).Contains(":\\"))
                    Settings.Default[property] = Path.Combine(appFolder, Settings.Default[property] as string);
        }
    }
}
