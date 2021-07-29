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
            get => InputTextBox.Text;
            set 
            {
                InputTextBox.Text = value;
                TypeCount++;

                if (TypeCount == 200)
                {
                    AllLessonTextBox.Text = AllLessonTextBox.Text.Remove(0, TypeCount);
                    InputTextBox.Text = "";

                    TypeCount = 0;
                }
            } 
        }

        public string LeftText
        {
            get;
            set;
        }

        public string AllText
        {
            get => AllLessonTextBox.Text;
            set => AllLessonTextBox.Text = value;
        }

        public string ErrorText
        {
            get;
            set;
        }

        public void LoadText(string text)
        {
            AllText = LeftText = text;
            DoneText = ErrorText = "";
            TypeCount = 0;
        }
    }
}
