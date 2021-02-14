using System;
using System.Linq;
using System.Windows;
using WPFMeteroWindow.Properties;
using CommandMakerLibrary;

namespace WPFMeteroWindow.Commands
{
    public class ColorSetter : ICommand
    {
        public string Name { get; set; } = "cl";
        
        public object Value { get; set; }
        
        public void Execute(object[] args)
        {
            var appDictionaries = Application.Current.Resources.MergedDictionaries;
            switch (args[0].ToString())
            {
                case "main":
                    SetColor.FirstColor(args[1].ToString());
                    break;
                
                case "secd":
                    SetColor.SecondColor(args[1].ToString());
                    break;
                
                case "clim":
                    SetColor.CommandLineFirstColor(args[1].ToString());
                    break;
                
                case "clis":
                    SetColor.CommandLineSecondColor(args[1].ToString());
                    break;
                
                case "kbbg":
                    SetColor.KeyboardBackground(args[1].ToString());
                    break;
                
                case "kbbr":
                    SetColor.KeyboardBorder(args[1].ToString());
                    break;
                
                case "kbhl":
                    SetColor.KeyboardHighlight(args[1].ToString());
                    break;
                
                case "t":
                    switch (args[1].ToString())
                    {
                        case "lt":
                            appDictionaries[appDictionaries.Count - 2] = new ResourceDictionary()
                            {
                                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute)
                            };
                            Settings.Default.ThemeResourceDictionary =
                                "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml";
                            break;
                
                        case "dt":
                            appDictionaries[appDictionaries.Count - 2] = new ResourceDictionary()
                            {
                                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml", UriKind.RelativeOrAbsolute)
                            };
                            Settings.Default.ThemeResourceDictionary =
                                "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml";
                            break;
                        
                        default:
                            var colorScheme = args[1].ToString();
                            try
                            {
                                appDictionaries[appDictionaries.Count - 3] = new ResourceDictionary()
                                {
                                    Source = new Uri(
                                        "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + colorScheme +
                                        ".xaml", UriKind.RelativeOrAbsolute)
                                };

                                Settings.Default.ColorSchemeResourceDictionary =
                                    "pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + colorScheme +
                                    ".xaml";
                            }
                            catch
                            {
                                appDictionaries[appDictionaries.Count - 3] = new ResourceDictionary()
                                {
                                    Source = new Uri(
                                        "pack://application:,,,/MahApps.Metro;component/Styles/Accents/steel.xaml", UriKind.RelativeOrAbsolute)
                                };

                                Settings.Default.ColorSchemeResourceDictionary =
                                    "pack://application:,,,/MahApps.Metro;component/Styles/Accents/steel.xaml";
                            }
                            break;
                    }
                    break;
            }
            
            Settings.Default.Save();
        }
    }
}