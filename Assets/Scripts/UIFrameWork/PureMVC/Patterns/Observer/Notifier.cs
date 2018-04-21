using System;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
    public class Notifier : INotifier
    {
        protected IFacade Facade
        {
            get
            {
                return Patterns.Facade.Instance;
            }
        }

        public string MultitonKey { get; protected set; }

        public virtual void SendNotification(string notificationName)
        {
            Facade.SendNotification(notificationName);
        }

        public virtual void SendNotification(string notificationName, object body)
        {
            Facade.SendNotification(notificationName, body);
        }

        public virtual void SendNotification(string notificationName, object body, string type)
        {
            Facade.SendNotification(notificationName, body, type);
        }

        public void InitializeNotifier(string key)
        {
            MultitonKey = key;
        }
    }
}
