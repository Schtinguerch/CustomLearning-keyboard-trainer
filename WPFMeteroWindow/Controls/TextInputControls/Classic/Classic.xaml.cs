using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WPFMeteroWindow.Controls;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Controls.TextInputControls.Classic
{
    /// <summary>
    /// Логика взаимодействия для Classic.xaml
    /// </summary>
    public partial class Classic : Page, ITextInputControl
    {
        private int TypeCount = 0;

        public Classic()
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
            get => InputRun.Text;
            set 
            {
                var availableLength = TypeCount % 200;
                InputRun.Text = value.Substring(value.Length - availableLength, availableLength);

                TypeCount++;
            } 
        }

        public string LeftText
        {
            get => LeftRun.Text;
            set => LeftRun.Text = value;
        }

        public string AllText
        {
            get;
            set;
        }

        public string ErrorText
        {
            get => MistakenRun.Text;
            set => MistakenRun.Text = value;
        }

        public void LoadText(string text)
        {
            AllText = LeftText = text;
            DoneText = ErrorText = "";
            TypeCount = 0;
        }
    }
}
