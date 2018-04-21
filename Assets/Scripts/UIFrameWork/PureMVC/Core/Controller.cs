using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace PureMVC.Core
{
    public class Controller : IController
    {
        private static IController instance = null;
        public static IController Instance
        {
            get 
            {
                if (instance == null)
                    instance = new Controller();
                return instance;
            }
        }

        public const string DEFAULT_KEY = "PureMVC";
        private readonly IDictionary<string, object> m_commandMap;
        private IView m_view;
        protected string m_multitonKey;

        
        public IEnumerable<string> ListNotificationNames
        {
            get { return m_commandMap.Keys; }
        }

        protected Controller()
        {
            m_multitonKey = DEFAULT_KEY;
            m_commandMap = new Dictionary<string, object>();
            InitializeController();
        }

        private void InitializeController()
        {
            m_view = View.Instance;
        } 

        public void ExecuteCommand(INotification notification)
        {
            if (!m_commandMap.ContainsKey(notification.Name)) return;
            var commandReference = m_commandMap[notification.Name];

            ICommand command;
            var commandType = commandReference as Type;
            if (commandType != null)
            {
                var commandInstance = Activator.CreateInstance(commandType);
                command = commandInstance as ICommand;
                if (command == null)
                    return;
            }
            else
            {
                command = commandReference as ICommand;
                if (command == null) return;
            }
            command.InitializeNotifier(m_multitonKey);
            command.Execute(notification);
        }

        public void RegisterCommand(string notificationName, Type commandType)
        {
            if (!m_commandMap.ContainsKey(notificationName))
            {
                m_view.RegisterObserver(notificationName, new Observer(ExecuteCommand, this));
            }

            m_commandMap[notificationName] = commandType;
        }

        public void RegisterCommand(string notificationName, ICommand command)
        {
            if (!m_commandMap.ContainsKey(notificationName))
            {
                m_view.RegisterObserver(notificationName, new Observer(ExecuteCommand, this));
            }
            command.InitializeNotifier(m_multitonKey);
            m_commandMap[notificationName] = command;
        }

        public bool HasCommand(string notificationName)
        {
            return m_commandMap.ContainsKey(notificationName);
        }

        public object RemoveCommand(string notificationName)
        {
            if (!m_commandMap.ContainsKey(notificationName)) return null;
            
            m_view.RemoveObserver(notificationName, this);
            var command = m_commandMap[notificationName];
            m_commandMap.Remove(notificationName);
            return command;
        }

        

        public void Dispose()
        {
            m_commandMap.Clear();
        }
    }
}
