using System;
using System.Windows;

namespace WPFMeteroWindow
{
    public static class XamlManager
    {
        private static string _baseFolder = "\\Resources\\dictionaries";
        private static string _dictionaryFile = "Styles.xaml";

        private static ResourceDictionary _dictionary =
            (ResourceDictionary)Application.LoadComponent(
                new Uri($"{_baseFolder}\\{_dictionaryFile}",
                UriKind.Relative));

        public static string DictionaryFile
        {
            get => _dictionaryFile;
            set
            {
                _dictionaryFile = value;
                _dictionary = (ResourceDictionary)
                    Application.LoadComponent(
                        new Uri($"{_baseFolder}\\{_dictionaryFile}", 
                        UriKind.Relative));
            }
        }

        public static T FindResource<T>(string key) 
            where T : class => 
            _dictionary[key] as T;
    }
}
