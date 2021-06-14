using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для ClickingGamePage.xaml
    /// </summary>
    public partial class ClickingGamePage : Page
    {
        private (string[][] keys, Button[] buttons) _keyboard;

        private GameManager _gameManager;

        private TappingCirclesDrawer _circlesDrawer;

        public ClickingGamePage()
        {
            InitializeComponent();

            var keyboardBinding = new Binding()
            {
                ElementName = nameof(KeyboardGrid),
                Path = new PropertyPath(Grid.ActualWidthProperty),
                Converter = new MathConverter(),
                ConverterParameter = "@VALUE/3",
            };

            KeyboardGrid.SetBinding(HeightProperty, keyboardBinding);
            
            _keyboard = KeyboardGrid.LoadButtons(Settings.Default.KeyboardLayoutFile);
            _gameManager = new GameManager(GameSceneCanvas, SoundTrackPlayer, Settings.Default.LoadedMapFolder);
            
            _circlesDrawer = new TappingCirclesDrawer(GameSceneCanvas, _keyboard);
            _gameManager.CirclesDrawer = _circlesDrawer;

            StartMap();
        }

        private void StartMap()
        {
            Settings.Default.MinTapperingAuraSize = 50d;
            _gameManager.StartGame();
        }

        private void ClickingGamePage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                PageManager.HidePages();
        }

        private void CloseGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageManager.HidePages();
        }

        private void RestartMapButton_OnClick(object sender, RoutedEventArgs e) =>
            StartMap();
    }
}
