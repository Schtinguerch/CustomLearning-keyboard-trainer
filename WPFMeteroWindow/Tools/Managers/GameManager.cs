using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using ScriptMaker;
using WPFMeteroWindow.Commands;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
    }

    public class GameManager
    {
        private List<string> filesFromDifficulty = new List<string>()
        {
            "easy.clmap",
            "normal.clmap",
            "hard.clmap",
        };

        private Difficulty _difficulty = Difficulty.Normal;

        private Queue<long> _tappingTimeQueue = new Queue<long>();
        
        private Canvas _gameCanvas;

        private MediaElement _player;

        private Script _gameScript;

        private TappingCirclesDrawer _drawer;

        private Stopwatch _timer;

        private DispatcherTimer _ticker;

        private bool _mediaLoaded;

        public TappingCirclesDrawer CirclesDrawer
        {
            set => _drawer = value;
        }

        public GameManager(Canvas gameCanvas, MediaElement player, string mapFolder)
        {
            if (!Directory.Exists(mapFolder))
            {
                LogManager.Log($"Open game map: \"{mapFolder}\" -> failed: file does not exist");
                throw new ArgumentException("File does not exist");
            }

            if (gameCanvas == null)
            {
                LogManager.Log($"Canvas load -> failed: empty canvas memory reference");
                throw new NullReferenceException("Null canvas");
            }

            _gameCanvas = gameCanvas;
            _player = player;

            var mapPreprocessor = new CommandProcessor(new List<Command>()
            {
                new PreDefine()
            });
            
            var mapProcessor = new CommandProcessor(new List<Command>()
            {
                new PlaySoundTrack(),
                new SetProperty(),
            }, player);

            var code = File.ReadAllText($"{mapFolder}\\{ChosenMapFile}");
            
            _gameScript = new Script(code, mapProcessor, mapPreprocessor);
            _gameScript.ExecutionEnded += EndGame;
        }

        public void StartGame()
        {
            _gameScript.ReturnToBeginning();
            _mediaLoaded = false;

            var mt = new DispatcherTimer();
            mt.Interval = TimeSpan.FromSeconds(3);
            mt.Tick += (s, e) =>
            {
                _mediaLoaded = true;
                mt.Stop();
            };

            mt.Start();

            _ticker = new DispatcherTimer();

            _ticker.Interval = TimeSpan.FromMilliseconds(1);
            _ticker.Tick += OnTick;

            _ticker.Start();
        }

        private void EndGame(object sender)
        {
            _timer.Stop();
            _ticker.Stop();
            
            _ticker = null;
            _timer = null;
            
            GC.Collect();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var command = _gameScript.PreparedCodeToRun();

            if (!command.Contains(" > "))
            {
                if (!_mediaLoaded) return;
                _gameScript.RunCurrentCommand();

                if (command.Contains("play"))
                { 
                    _timer = Stopwatch.StartNew();
                    _player.Play();
                }
                
                return;
            }

            var tokens = command.Split(new[] {" > "}, StringSplitOptions.RemoveEmptyEntries);
            var currentTapTime = tokens[0].ToMilliseconds();

            if (_timer.ElapsedMilliseconds - 500 >= 1 * (currentTapTime - Settings.Default.TapperingMilliseconds))
            {
                _gameScript.RunCurrentCommand();
                var character = tokens[1];
                
                _drawer?.DrawCicle(character);
                
                _tappingTimeQueue.Enqueue(currentTapTime);
                return;
            }

            if (_tappingTimeQueue.Count > 0)
                if (_timer.ElapsedMilliseconds - 500> _tappingTimeQueue.Peek() + Settings.Default.TapperingMilliseconds / 6d)
                {
                    TryDequeueTappingTime();
                    return;
                }
        }

        private void TryDequeueTappingTime()
        {
            if (_tappingTimeQueue.Count == 0) return;
            
            _tappingTimeQueue.Dequeue();
            if (_tappingTimeQueue.Count != 0) return;
            
            _gameCanvas.Children.Clear();
            GC.Collect();
        }

        private string ChosenMapFile =>
            filesFromDifficulty[(int) _difficulty];
    }
}