using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using WPFMeteroWindow.Properties;
using WPFMeteroWindow.Controls;
using Newtonsoft.Json;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ActiveUsingConfigs.xaml
    /// </summary>
    public partial class ActiveUsingConfigs : Page
    {
        private List<string> _previousConfigsData;

        public ActiveUsingConfigs()
        {
            InitializeComponent();
            ReinitializeSkinList();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
           PageManager.HidePages();

        private void ReinitializeSkinList()
        {
            ScrollListStackPanel.Children.Clear();
            _previousConfigsData = AppManager.JsonReadData<List<string>>(Settings.Default.RecentConfigs);

            if (_previousConfigsData.Count > 0)
                EmptyListTextBox.Visibility = Visibility.Hidden;

            foreach (var layout in _previousConfigsData)
                if (File.Exists(layout) && File.ReadAllText(layout).Contains("#config"))
                    ScrollListStackPanel.Children.Insert(0, new ConfigItem(layout)
                    {
                        Margin = new Thickness(0, 20, 0, 0)
                    });
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfigManager.ImportConfigViaExplorer();
            var newRecentConfigsData = AppManager.JsonReadData<List<string>>(Settings.Default.RecentConfigs);

            if (newRecentConfigsData.Count == _previousConfigsData.Count)
                return;

            ReinitializeSkinList();
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollListStackPanel.Children.Clear();
            _previousConfigsData?.Clear();

            File.WriteAllText(Settings.Default.RecentConfigs, "[]");
            EmptyListTextBox.Visibility = Visibility.Visible;
        }

        private void wrapPanel_DragEnter(object sender, DragEventArgs e)
        {
            DragAndDropBackground.Opacity = 0.5d;

            EmptyListTextBox.VerticalAlignment = VerticalAlignment.Bottom;
            EmptyListTextBox.Text = Localization.uLoadNewStyles;
            EmptyListTextBox.Visibility = Visibility.Visible;
        }

        private void wrapPanel_DragLeave(object sender, DragEventArgs e)
        {
            DragAndDropBackground.Opacity = 0d;

            EmptyListTextBox.VerticalAlignment = VerticalAlignment.Center;
            EmptyListTextBox.Text = Localization.uEmptyList;
            EmptyListTextBox.Visibility = Visibility.Hidden;
        }

        private void wrapPanel_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                wrapPanel_DragLeave(null, null);
                return;
            }
                

            var configs = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (configs == null)
            {
                wrapPanel_DragLeave(null, null);
                return;
            }
                

            foreach (var config in configs)
            {
                try
                {
                    UserConfigManager.AddToRecent(config);
                }
                catch
                {
                    //Now, thinking
                }
            }

            ReinitializeSkinList();
            wrapPanel_DragLeave(null, null);
        }
    }
}
