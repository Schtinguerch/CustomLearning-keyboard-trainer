using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using WPFMeteroWindow.Properties;

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
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }
    }
}
