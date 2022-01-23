using System;
using System.Globalization;
using System.Threading;
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

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
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
            MessageBox.Show($"{Localization.uError}", $"{message}");
        }
    }
}
