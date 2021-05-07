using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFMeteroWindow.Properties;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;
using Path = System.IO.Path;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для KeyboardLayoutEditorPage.xaml
    /// </summary>
    public partial class KeyboardLayoutEditorPage : Page
    {
        private KeyboardLayoutEditor _editor;

        private (string[][] keys, Button[] buttons) _keyboard;

        private int _choosenButtonIndex;

        public KeyboardLayoutEditorPage()
        {
            InitializeComponent();
            Intermediary.LayoutPage = this;
            OpenLayoutButton.Focus();

            _editor = new KeyboardLayoutEditor(Settings.Default.KeyboardLayoutFile);
            _keyboard = KeyboardGrid.LoadButtons(Settings.Default.KeyboardLayoutFile);

            InitiateContextMenu();

            EditorTitleTextBox.Text = $"{Path.GetFileName(Settings.Default.KeyboardLayoutFile)} - {Localization.uKbLayoutEditor}";
            Intermediary.RichPresentManager.Update("Keyboard layout editor", "Editing layout...", "");

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                    PageManager.HidePages();
            };
        }

        private void InitiateContextMenu()
        {
            for (int i = 0; i < 61; i++)
            {
                _keyboard.buttons[i].ContextMenu = null;

                var contextMenu = new ContextMenu()
                {
                    Margin = new Thickness(0d),
                    Padding = new Thickness(0d),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.SecondBackground)),
                    HasDropShadow = false,
                };

                contextMenu.Items.Add(new KeyboardLayoutEditorMenu(_keyboard.buttons[i], _keyboard.keys[i], i));

                contextMenu.Opened += (s, e) =>
                {
                    (contextMenu.Items[0] as KeyboardLayoutEditorMenu).ChoosenButton.Background
                        = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(Settings.Default.KeyboardHighlightColor));
                };

                contextMenu.Closed += (s, e) =>
                {
                    (contextMenu.Items[0] as KeyboardLayoutEditorMenu).ChoosenButton.Background
                        = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(Settings.Default.KeyboardBackgroundColor));

                    _keyboard.keys[(contextMenu.Items[0] as KeyboardLayoutEditorMenu).Index] =
                        (contextMenu.Items[0] as KeyboardLayoutEditorMenu).ButtonCharacters;
                };

                _keyboard.buttons[i].ContextMenu = contextMenu;
            }
        }

        private void DisplayDataFromEditor()
        {
            _keyboard.keys = _editor.LayoutKeys;
            for (int i = 0; i < 61; i++)
                KeyboardLoader.SetContent(_keyboard.buttons[i], _keyboard.keys[i]);
        }

        private void NewLayout()
        {
            _editor = new KeyboardLayoutEditor();
            DisplayDataFromEditor();

            InitiateContextMenu();
            EditorTitleTextBox.Text = $"{Localization.uNewKbLayout} - {Localization.uKbLayoutEditor}";
        }

        private void OpenLayout()
        {
            var openDialog = new OpenFileDialog()
            {
                DefaultExt = ".lml",
                Filter = "LML-Files|*.lml",
                Multiselect = false
            };

            if (openDialog.ShowDialog() == true)
            {
                _editor = new KeyboardLayoutEditor(openDialog.FileName);
                DisplayDataFromEditor();

                InitiateContextMenu();
                EditorTitleTextBox.Text = Path.GetFileName(openDialog.FileName) + $" - {Localization.uKbLayoutEditor}";
            }
        }

        private void SaveLayout()
        {
            _editor.LayoutKeys = _keyboard.keys;
            _editor.WriteDataOnFile();
        }

        private void NewLayoutButton_OnClick(object sender, RoutedEventArgs e) =>
            NewLayout();

        private void OpenLayoutButton_OnClick(object sender, RoutedEventArgs e) =>
            OpenLayout();

        private void SaveLayoutButton_OnClick(object sender, RoutedEventArgs e) =>
            SaveLayout();

        private void CancelLayoutButton_OnClick(object sender, RoutedEventArgs e) =>
            PageManager.HidePages();
    }
}
