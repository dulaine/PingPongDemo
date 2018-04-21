using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace PureMVC.Core
{
    public class View : IView
    {
        private static IView instance = null;
        public static IView Instance
        {
            get
            {
                if (instance == null)
                    instance = new View();
                return instance;
            }
        }

        public const string DEFAULT_KEY = "PureMVC";
        protected string m_multitonKey;
        protected IDictionary<string, IMediator> m_mediatorMap;
        protected IDictionary<string, IList<IObserver>> m_observerMap;

        public IEnumerable<string> ListMediatorNames
        {
            get { return m_mediatorMap.Keys; }
        }

        protected View()
        {
            m_multitonKey = DEFAULT_KEY;
            m_mediatorMap = new Dictionary<string, IMediator>();
            m_observerMap = new Dictionary<string, IList<IObserver>>();
            InitializeView();
        }

        protected virtual void InitializeView()
        {

        }


        public virtual void RegisterObserver(string notificationName, IObserver observer)
        {
            if (!m_observerMap.ContainsKey(notificationName))
            {
                m_observerMap[notificationName] = new List<IObserver>();
            }

            m_observerMap[notificationName].Add(observer);
        }

        public virtual void NotifyObservers(INotification notification)
        {
            IList<IObserver> observers = null;

            if (m_observerMap.ContainsKey(notification.Name))
            {
                IList<IObserver> observers_ref = m_observerMap[notification.Name];
                observers = new List<IObserver>(observers_ref);
            }

            if (observers == null) return;

            try
            {
                foreach (IObserver observer in observers)
                {
                    observer.NotifyObserver(notification);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("接受消息有误：" + notification.Name);
                UnityEngine.Debug.LogError("Error = " + e.ToString());
            }

            
        }

        public virtual void RemoveObserver(string notificationName, object notifyContext)
        {
            if (!m_observerMap.ContainsKey(notificationName)) return;
            var observers = m_observerMap[notificationName];

            lock (observers)
            {
                for (var i = 0; i < observers.Count; i++)
                {
                    if (!observers[i].CompareNotifyContext(notifyContext)) continue;
                    observers.RemoveAt(i);
                    break;
                }

                if (observers.Count == 0)
                    m_observerMap.Remove(notificationName);
            }
        }

        public virtual void RegisterMediator(IMediator mediator)
        {
            lock (m_mediatorMap)
            {
                if (m_mediatorMap.ContainsKey(mediator.MediatorName)) return;

                mediator.InitializeNotifier(m_multitonKey);
                m_mediatorMap[mediator.MediatorName] = mediator;

                var interests = mediator.ListNotificationInterests;

                IObserver observer = new Observer(mediator.HandleNotification, mediator);

                foreach (var t in interests)
                {
                    RegisterObserver(t, observer);
                }
            }

            mediator.OnRegister();
        }

        public virtual IMediator RetrieveMediator(string mediatorName)
        {
            if (!m_mediatorMap.ContainsKey(mediatorName)) return null;
            return m_mediatorMap[mediatorName];
        }

        public virtual IMediator RemoveMediator(string mediatorName)
        {
            lock (m_mediatorMap)
            {
                if (!m_mediatorMap.ContainsKey(mediatorName)) return null;
                var mediator = m_mediatorMap[mediatorName];

                var interests = mediator.ListNotificationInterests;

                foreach (var t in interests)
                {
                    RemoveObserver(t, mediator);
                }

                m_mediatorMap.Remove(mediatorName);

                mediator.OnRemove();
                return mediator;
            }
        }

        public virtual bool HasMediator(string mediatorName)
        {
            return m_mediatorMap.ContainsKey(mediatorName);
        }

        public void Dispose()
        {
            m_observerMap.Clear();
            m_mediatorMap.Clear();
        }
    }
}
