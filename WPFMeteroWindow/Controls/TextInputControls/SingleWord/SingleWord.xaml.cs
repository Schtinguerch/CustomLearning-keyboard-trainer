using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WPFMeteroWindow.Controls;

namespace WPFMeteroWindow.Controls.TextInputControls.SingleWord
{
    /// <summary>
    /// Логика взаимодействия для SingleWord.xaml
    /// </summary>
    public partial class SingleWord : Page, ITextInputControl
    {
        private List<string> _words;
        private int _selectedWordIndex = 0;

        public SingleWord()
        {
            InitializeComponent();

            LessonManager.TextInputPresenter.TextInputWay = this;
            AppManager.InitializeApplication();
        }

        public string DoneText
        {
            get => SingleWordInputTextBox.Text;
            set => SingleWordInputTextBox.Text = value;
        }

        private string _leftText;
        public string LeftText 
        {  
            get => _leftText;
            set
            {
                _leftText = value;
                if (value.Length == 0) return;

                if (value[0] == ' ')
                {
                    DoneText = "";
                    LessonManager.LeftRoad = LessonManager.LeftRoad.Substring(1, LessonManager.LeftRoad.Length - 1);

                    if (_selectedWordIndex == _words.Count - 1) return;
                    AllText = _words[++_selectedWordIndex];
                }
            }
        }

        public string AllText
        {
            get => SingleWordTextBox.Text;
            set => SingleWordTextBox.Text = value; 
        }
        public string ErrorText { get; set; }

        public void LoadText(string text)
        {
            LeftText = text;
            DoneText = ErrorText = "";

            _words = LeftText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _selectedWordIndex = 0;

            AllText = _words[_selectedWordIndex];
        }
    }
}
