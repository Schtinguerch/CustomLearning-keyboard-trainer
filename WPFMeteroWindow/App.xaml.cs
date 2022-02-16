using System;
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
        public void LoadUserResourceDictionaries()
        {
            var appDictionary = Application.Current.Resources.MergedDictionaries;
            appDictionary[appDictionary.Count - 2] = new ResourceDictionary()
            {
                Source = new Uri(Settings.Default.ThemeResourceDictionary, UriKind.RelativeOrAbsolute)
            };
            appDictionary[appDictionary.Count - 3] = new ResourceDictionary()
            {
                Source = new Uri(Settings.Default.ColorSchemeResourceDictionary, UriKind.RelativeOrAbsolute)
            };
        }
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            LoadUserResourceDictionaries();
            SetGlobalTryCatch();

            LanguageManager.SetLanguage(Settings.Default.ChosenLanguageIndex);

            if (!Directory.Exists("DefaultData"))
            {
                var downloadingWindow = new DownloadingMissingPacksWindow();
                downloadingWindow.Show();

                var client = new WebClient();
                var zipArchiveFilename = "Packages.zip";

                client.DownloadFile("https://github.com/Schtinguerch/schtinguerch.github.io/raw/master/MissingPackages/AppEssentials.zip", zipArchiveFilename);

                ZipFile.ExtractToDirectory(zipArchiveFilename, System.AppDomain.CurrentDomain.BaseDirectory);
                ZipFile.ExtractToDirectory(zipArchiveFilename, @"C:\Program Files (x86)\CustomLearning");

                System.Threading.Thread.Sleep(500);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
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
    }
}
