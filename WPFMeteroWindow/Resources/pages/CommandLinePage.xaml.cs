using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using WPFMeteroWindow.Commands;
using WPFMeteroWindow.Properties;
using CommandManager = CommandMakerLibrary.CommandManager;
using ICommand = CommandMakerLibrary.ICommand;

namespace WPFMeteroWindow.Resources.pages
{
    /// <summary>
    /// Логика взаимодействия для CommandLinePage.xaml
    /// </summary>
    public partial class CommandLinePage : Page
    {
        private CommandManager _commandManager = new CommandManager();
        
        private List<ICommand> _commands = new List<ICommand>()
        {
            new FileOpener(),
            new FontSetter(),
            new ColorSetter(),
        };

        public CommandLinePage()
        {
            InitializeComponent();
            CommandTextBox.Focus();
            
            _commandManager.AddCommands(_commands);
            Intermediary.RichPresentManager.Update("Command line", "App management...", "");
        }

        private string AfterProcessedMacroses()
        {
            var text = CommandTextBox.Text;

            if (text.Contains("!save"))
            {
                Settings.Default.Save();
                text = text.Replace("!save", "");
            }
            else if (text.Contains("!discard"))
            {
                Settings.Default.Reload();
                text = text.Replace("!discard", "");

                SetUnAutoColors();
            }
            else if (text.Contains("!default"))
            {
                Settings.Default.Reset();
                text = text.Replace("!default", "");

                SetUnAutoColors();
            }

            return text;
        }

        private void SetUnAutoColors()
        {
            KeyboardManager.LoadKeyboardData(Settings.Default.KeyboardLayoutFile);
            SetFont.MainLetters(Settings.Default.LessonLettersFont);

            SetColor.WindowColor(Settings.Default.ColorSchemeResourceDictionary.Split(new[] { '/' }).Last()
                .Split(new[] { '.' })[0]);
            SetColor.ColorScheme(Settings.Default.ThemeResourceDictionary.Contains("BaseLight") ? Theme.Light : Theme.Dark);
        }

        private void CommandLinePage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommandTextBox.Text = AfterProcessedMacroses();
                
                var expressions = CommandTextBox.Text.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var expression in expressions)
                {
                    var tokens =
                        expression.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();

                    var commandName = tokens[0];
                    tokens.RemoveAt(0);

                    var arguments = tokens.ToArray();
                    _commandManager.ExecuteCommand(commandName, arguments);
                }
                
                PageManager.HidePages();
            }

            else if (e.Key == Key.Escape)
                PageManager.HidePages();
        }
    }
}
