using System;
using System.Reflection;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
    [Serializable]
    public class Observer : IObserver
    {
        public Action<INotification> NotifyMethod { private get; set; }
        public object NotifyContext { private get; set; }

        public Observer(Action<INotification> notifyMethod, object notifyContext)
        {
            NotifyMethod = notifyMethod;
            NotifyContext = notifyContext;
        }

        public void NotifyObserver(INotification notification)
        {
            Action<INotification> method;
            //object context;

            lock (this)
            {
                method = NotifyMethod;
                //context = NotifyContext;
            }

            method.Invoke(notification);

            //var t = context.GetType();
            //const BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            //var mi = t.GetMethod(NotifyMethod, f);
            //mi.Invoke(context, new object[] { notification });
        }

        public bool CompareNotifyContext(object obj)
        {
            lock (this)
            {
                return NotifyContext.Equals(obj);
            }
        }

    }
}
