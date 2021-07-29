using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Controls.TextInputControls.SingleLineWithStaticCaret
{
    /// <summary>
    /// Логика взаимодействия для SingleLineWithStaticCaret.xaml
    /// </summary>
    public partial class SingleLineWithStaticCaret : Page, ITextInputControl
    {
        public SingleLineWithStaticCaret()
        {
            InitializeComponent();

            LessonManager.TextInputPresenter.TextInputWay = this;

            if (Settings.Default.IsFirstTextInputOpen)
            {
                Settings.Default.IsFirstTextInputOpen = false;
                AppManager.InitializeApplication();
            }

            Intermediary.App.RestartLesson();
        }

        public string DoneText
        {
            get => inputTextBox.Text;
            set => inputTextBox.Text = value;
        }

        public string LeftText
        {
            get => inputTextBlock.Text;
            set => inputTextBlock.Text = value;
        }

        public string AllText { get; set; }
        public string ErrorText
        {
            get => errorInputTextBlock.Text;
            set => errorInputTextBlock.Text = value;
        }

        public void LoadText(string text)
        {
            AllText = LeftText = text;
            DoneText = ErrorText = "";
        }
    }
}
