using System;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
    public class DelegateCommand : Notifier, ICommand
    {
        public DelegateCommand(Action<INotification> action)
        {
            m_action = action;
        }

        public virtual void Execute(INotification notification)
        {
            m_action(notification);
        }

        private readonly Action<INotification> m_action;
    }
}
