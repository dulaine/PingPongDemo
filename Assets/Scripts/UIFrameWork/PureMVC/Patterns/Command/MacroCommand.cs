using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
    public class MacroCommand : Notifier, ICommand
    {
        private readonly IList<object> m_subCommands;


        public MacroCommand()
        {
            m_subCommands = new List<object>();
            InitializeMacroCommand();
        }

        public void Execute(INotification notification)
        {
            while (m_subCommands.Count > 0)
            {
                var commandType = m_subCommands[0] as Type;
                if (commandType != null)
                {
                    var commandInstance = Activator.CreateInstance(commandType);

                    if (commandInstance is ICommand)
                    {
                        var command = (ICommand)commandInstance;
                        command.InitializeNotifier(MultitonKey);
                        command.Execute(notification);
                    }
                }
                else
                {
                    var command = m_subCommands[0] as ICommand;
                    if (command != null)
                    {
                        command.InitializeNotifier(MultitonKey);
                        command.Execute(notification);
                    }
                }

                m_subCommands.RemoveAt(0);
            }
        }

        protected virtual void InitializeMacroCommand()
        {
        }

        protected void AddSubCommand(Type commandType)
        {
            m_subCommands.Add(commandType);
        }

        protected void AddSubCommand(ICommand command)
        {
            m_subCommands.Add(command);
        }

        
    }
}
