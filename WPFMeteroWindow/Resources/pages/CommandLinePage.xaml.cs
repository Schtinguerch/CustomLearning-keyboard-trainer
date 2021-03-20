using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using WPFMeteroWindow.Commands;
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
        }

        private void CommandLinePage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
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
        }
    }
}
