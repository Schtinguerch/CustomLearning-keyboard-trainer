using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFMeteroWindow.Properties;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для KeyboardLayoutEditorMenu.xaml
    /// </summary>
    public partial class KeyboardLayoutEditorMenu : UserControl
    {
        private Button _choosenButton;

        public Button ChoosenButton
        {
            get => _choosenButton;
        }

        private string[] _buttonCharacters;

        public string[] ButtonCharacters
        {
            get => _buttonCharacters;
        }

        private int _index;

        public int Index
        {
            get => _index;
        }

        public KeyboardLayoutEditorMenu(Button choosenButton, string[] buttonCharacters, int index)
        {
            InitializeComponent();

            _choosenButton = choosenButton;
            _buttonCharacters = buttonCharacters;

            LoadCharacters();
            DefaultKeyTextBox.Focus();

            _index = index;
        }

        public void LoadCharacters()
        {
            DefaultKeyTextBox.Text = _buttonCharacters[0];
            ShiftKeyTextBox.Text = _buttonCharacters[1];
            AltGrKeyTextBox.Text = _buttonCharacters[2];
            ShiftAndAltGrKeyTextBox.Text = _buttonCharacters[3];
        }

        public void UpdateButtonKey()
        {
            KeyboardLoader.SetContent(
                _choosenButton,
                DefaultKeyTextBox.Text,
                ShiftKeyTextBox.Text,
                AltGrKeyTextBox.Text,
                ShiftAndAltGrKeyTextBox.Text);

            _buttonCharacters[0] = DefaultKeyTextBox.Text;
            _buttonCharacters[1] = ShiftKeyTextBox.Text;
            _buttonCharacters[2] = AltGrKeyTextBox.Text;
            _buttonCharacters[3] = ShiftAndAltGrKeyTextBox.Text;
        }

        private void KeyboardLayoutEditorMenu_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UpdateButtonKey();
        }
    }
}
