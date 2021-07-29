using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WPFMeteroWindow.Controls;

namespace WPFMeteroWindow
{
    public enum TextInputControl
    {
        Classic,
        SingleLineWithStaticCaret,
        SingleWord,
    }

    public class LessonTextInputPresenter
    {
        private ITextInputControl _textInputControl;

        private TextInputControl _chosenTextInputControl;

        private Dictionary<TextInputControl, string> _textInputControls = new Dictionary<TextInputControl, string>() 
        {
            { TextInputControl.Classic, "Classic" },
            { TextInputControl.SingleLineWithStaticCaret, "SingleLineWithStaticCaret" },
            { TextInputControl.SingleWord, "SingleWord" },
        };

        public ITextInputControl TextInputWay
        {
            get => _textInputControl;
            set => _textInputControl = value;
        }

        public Frame TextInputFrame { get; set; }

        public string DoneText
        {
            get => _textInputControl.DoneText;
            set => _textInputControl.DoneText = value;
        }

        public string LeftText
        {
            get => _textInputControl.LeftText;
            set => _textInputControl.LeftText = value;
        }

        public string ErrorText
        {
            get => _textInputControl.ErrorText;
            set => _textInputControl.ErrorText = value;
        }

        public TextInputControl TextInputControl
        {
            get => _chosenTextInputControl;
            set
            {
                var baseFolder = "Controls/TextInputControls";
                var pageName = _textInputControls[value];

                TextInputFrame.Source = new Uri($"{baseFolder}/{pageName}/{pageName}.xaml", UriKind.Relative);
                //_textInputControl = (ITextInputControl)TextInputFrame.Content();

                _chosenTextInputControl = value;
            }
        }
        
        public void LoadText(string text) =>
            _textInputControl.LoadText(text);
    }
}
