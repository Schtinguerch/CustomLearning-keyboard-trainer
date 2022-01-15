using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Properties;

using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ActiveUsingLayoutsPage.xaml
    /// </summary>
    public partial class ActiveUsingLayoutsPage : Page
    {
        private List<string> _recentLayoutData;

        public ActiveUsingLayoutsPage()
        {
            InitializeComponent();
            ReinitializeRecentLayoutList();
        }
        private void ReinitializeRecentLayoutList()
        {
            ScrollListStackPanel.Children.Clear();
            _recentLayoutData = AppManager.JsonReadData<List<string>>(Settings.Default.RecentLayoutsPath);

            if (_recentLayoutData.Count > 0)
                EmptyListTextBox.Visibility = Visibility.Hidden;

            foreach (var layout in _recentLayoutData)
                InsertNewLayout(layout);
        }

        private void InsertNewLayout(string layoutFilename) =>
            ScrollListStackPanel.Children.Insert(0, new KeyboardLayoutItem()
            {
                LayoutPath = layoutFilename,
                Margin = new Thickness(0, 20, 0, 0),
            });

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            PageManager.HidePages();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Opener.NewKeyboardLayoutViaExplorer(Settings.Default.CurrentLayout);
            var newRecentCourcesData = AppManager.JsonReadData<List<string>>(Settings.Default.RecentLayoutsPath);

            if (newRecentCourcesData.Count == _recentLayoutData.Count)
                return;

            ReinitializeRecentLayoutList();
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollListStackPanel.Children.Clear();
            _recentLayoutData?.Clear();

            File.WriteAllText(Settings.Default.RecentLayoutsPath, "[]");
            EmptyListTextBox.Visibility = Visibility.Visible;
        }

        private void wrapPanel_DragEnter(object sender, DragEventArgs e)
        {
            DragAndDropBackground.Opacity = 0.5d;

            EmptyListTextBox.VerticalAlignment = VerticalAlignment.Bottom;
            EmptyListTextBox.Text = Localization.uLoadNewLayouts;
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
                return;

            var cources = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (cources == null)
                return;

            foreach (var course in cources)
                Opener.NewKeyboardLayout(course);

            ReinitializeRecentLayoutList();
            wrapPanel_DragLeave(null, null);
        }
    }
}
