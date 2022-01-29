using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Controls.TextInputControls.Competitive
{
    /// <summary>
    /// Логика взаимодействия для Competitive.xaml
    /// </summary>
    public partial class Competitive : Page, ITextInputControl
    {
        private string _doneText, _leftText;
        private int _highlightingRunIndex, _highlightingTextBlockIndex;
        private int _startCheckIndex = 0;

        private List<Run> _chacacterRuns = new List<Run>(capacity: 1000);
        private List<TextBlock> _textBlocks = new List<TextBlock>(capacity: 200);

        private Brush _highlightBrush, _baseBrush;

        private Style _textBlockStyle = new Style(typeof(TextBlock));
        private Style _highlightedTextBlockStyle = new Style(typeof(TextBlock));

        private int HighlightingRunIndex
        {
            get => _highlightingRunIndex;
            set
            {
                _highlightingRunIndex = value;
                HighlightRun(value);
            }
        }

        private int HighlightingTextBlockIndex
        {
            get => _highlightingTextBlockIndex;
            set
            {
                _highlightingTextBlockIndex = value;
                HighlightWord(value);
            }
        }

        public string DoneText
        {
            get => _doneText;
            set
            {
                _doneText = value;
                HighlightingRunIndex = value.Length;

                if (!string.IsNullOrEmpty(value) && value.Last() == ' ')
                {
                    HighlightingTextBlockIndex += 1;
                    double jumpY = ErrorInputTextBlock.ActualHeight * 2;
                    
                    if (RelativePoint(_textBlocks[HighlightingTextBlockIndex], TextInputWrapPanel).Y > jumpY)
                    {
                        int topCount = 0;
                        for (int i = _startCheckIndex; i < _textBlocks.Count; i++)
                        {
                            if (RelativePoint(_textBlocks[i], TextInputWrapPanel).Y <= jumpY)
                                topCount++;
                            else
                            {
                                _startCheckIndex = i;
                                break;
                            }
                                
                        }

                        TextInputWrapPanel.Children.RemoveRange(0, topCount);
                    }
                }

                if (HighlightingRunIndex > 0)
                {
                    var point = RunPosition(_chacacterRuns[HighlightingRunIndex], _textBlocks[HighlightingTextBlockIndex], TextInputWrapPanel);
                    Canvas.SetLeft(ErrorInputTextBlock, point.X);
                    Canvas.SetTop(ErrorInputTextBlock, point.Y);
                }
            }
        }

        public string LeftText
        {
            get => _leftText;
            set => _leftText = value;
        }

        public string AllText { get; set; }
        public string ErrorText
        {
            get => ErrorInputTextBlock.Text;
            set
            {
                ErrorInputTextBlock.Text = value;
                ErrorInputTextBlock.Visibility = string.IsNullOrEmpty(value) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public Competitive()
        {
            InitializeComponent();

            var dictionary = (ResourceDictionary)Application.LoadComponent(new Uri("\\Resources\\dictionaries\\Styles.xaml", UriKind.Relative));
            _textBlockStyle = (Style)dictionary["CompetitiveTextStyle"];
            _highlightedTextBlockStyle = (Style)dictionary["HighlightedCompititiveTextStyle"];

            LessonManager.TextInputPresenter.TextInputWay = this;
            if (Settings.Default.IsFirstTextInputOpen)
            {
                Settings.Default.IsFirstTextInputOpen = false;
                AppManager.InitializeApplication();
            }

            Intermediary.App.RestartLesson();
        }

        public void LoadText(string text)
        {
            AllText = LeftText = text;
            DoneText = ErrorText = "";

            LoadRuns(text);
        }

        private void LoadRuns(string text)
        {
            _highlightBrush = (SolidColorBrush)
                new BrushConverter().ConvertFromString(Settings.Default.KeyboardHighlightColor);

            _baseBrush = (SolidColorBrush)
                new BrushConverter().ConvertFromString(Settings.Default.MainFontColor);

            MagicAllocation(text);

            HighlightingRunIndex = 0;
            HighlightingTextBlockIndex = 0;
        }

        private void MagicAllocation(string text)
        {
            foreach (var textBlock in _textBlocks)
                textBlock.Style = _textBlockStyle;

            foreach (var run in _chacacterRuns)
                run.TextDecorations = null;

            if (text.Length > _chacacterRuns.Count)
            {
                int offset = text.Length - _chacacterRuns.Count;
                for (int i = 0; i < offset; i++) _chacacterRuns.Add(new Run());
            }

            else
            {
                int offset = _chacacterRuns.Count - text.Length;
                _chacacterRuns.RemoveRange(0, offset);
            }

            var words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > _textBlocks.Count)
            {
                int offset = words.Length - _textBlocks.Count;
                for (int i = 0; i < offset; i++)
                {
                    var textBlock = new TextBlock() { Style = _textBlockStyle };

                    _textBlocks.Add(textBlock);
                }
            }

            else
            {
                int offset = _textBlocks.Count - words.Length;

                _textBlocks.RemoveRange(0, offset);
            }

            InitializeTextBlocks(ref words);
        }

        private void InitializeTextBlocks(ref string[] words)
        {
            _startCheckIndex = 0;
            TextInputWrapPanel.Children?.Clear();

            foreach (var textBlock in _textBlocks)
            {
                textBlock.Inlines?.Clear();
                TextInputWrapPanel.Children.Add(textBlock);
            }

            int textBlockIndex = 0, runIndex = 0;
            
            try
            {
                foreach (var word in words)
                {
                    foreach (var character in word)
                    {
                        _chacacterRuns[runIndex].Text = character.ToString();
                        _textBlocks[textBlockIndex].Inlines.Add(_chacacterRuns[runIndex]);
                        runIndex++;
                    }

                    _chacacterRuns[runIndex].Text = " ";
                    _textBlocks[textBlockIndex].Inlines.Add(_chacacterRuns[runIndex]);

                    runIndex++;
                    textBlockIndex++;
                }
            } 
            
            catch
            {

            }
        }

        private void HighlightRun(int index)
        {
            if (_chacacterRuns == null || _chacacterRuns.Count == 0 || index == _chacacterRuns.Count)
                return;

            _chacacterRuns[index].TextDecorations = TextDecorations.Underline;
            if (index == 0) return;

            _chacacterRuns[index - 1].TextDecorations = null;
        }

        private void HighlightWord(int index)
        {
            if (_textBlocks == null || _textBlocks.Count == 0 || index == _textBlocks.Count)
                return;

            _textBlocks[index].Style = _highlightedTextBlockStyle;
            if (index == 0) return;

            _textBlocks[index - 1].Style = _textBlockStyle;
        }

        private Point RelativePoint(UIElement element, UIElement relativeTo) =>
            element.TransformToAncestor(relativeTo).Transform(new Point(0, 0));

        private Point RunPosition(Run run, TextBlock textBlock, UIElement relativeTo)
        {
            double posLeft = run.ElementStart.GetCharacterRect(LogicalDirection.Forward).Left;
            double posTop = run.ElementStart.GetCharacterRect(LogicalDirection.Forward).Top;

            var relativeToTextBlockPoint = new Point(posLeft, posTop);
            var textBlockPoint = RelativePoint(textBlock, relativeTo);

            return new Point(
                relativeToTextBlockPoint.X + textBlockPoint.X,
                relativeToTextBlockPoint.Y + textBlockPoint.Y);
        }
    }
}
