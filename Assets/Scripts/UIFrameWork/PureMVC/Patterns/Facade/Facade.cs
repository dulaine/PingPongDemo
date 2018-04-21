using System;
using System.Collections.Generic;
using PureMVC.Core;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
    public class Facade : Notifier, IFacade
    {
        private static IFacade instance = null;
        public static IFacade Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new Facade();
                }
                return instance;
            }
        }
        public const string DEFAULT_KEY = "PureMVC";

        protected IController m_controller;
        protected IModel m_model;
        protected IView m_view;

        private Facade()
        {
            InitializeNotifier(DEFAULT_KEY);
            InitializeFacade();
        }

        protected virtual void InitializeFacade()
        {
            InitializeModel();
            InitializeController();
            InitializeView();
        }

        protected virtual void InitializeController()
        {
            if (m_controller != null) return;
            m_controller = Controller.Instance;
        }

        protected virtual void InitializeModel()
        {
            if (m_model != null) return;
            m_model = Model.Instance;
        }

        protected virtual void InitializeView()
        {
            if (m_view != null) return;
            m_view = View.Instance;
        }

        //============================================代理============================================
        public void RegisterProxy(IProxy proxy)
        {
            m_model.RegisterProxy(proxy);
        }

        public IProxy RetrieveProxy(string proxyName)
        {
            return m_model.RetrieveProxy(proxyName);
        }

        public IProxy RemoveProxy(string proxyName)
        {
            return m_model.RemoveProxy(proxyName);
        }

        public bool HasProxy(string proxyName)
        {
            return m_model.HasProxy(proxyName);
        }

        //============================================命令============================================
        public void RegisterCommand(string notificationName, Type commandType)
        {
            m_controller.RegisterCommand(notificationName, commandType);
        }

        public void RegisterCommand(string notificationName, ICommand command)
        {
            m_controller.RegisterCommand(notificationName, command);
        }

        public object RemoveCommand(string notificationName)
        {
            return m_controller.RemoveCommand(notificationName);
        }

        public bool HasCommand(string notificationName)
        {
            return m_controller.HasCommand(notificationName);
        }

        //============================================中介============================================
        public void RegisterMediator(IMediator mediator)
        {
            m_view.RegisterMediator(mediator);
        }

        public IMediator RetrieveMediator(string mediatorName)
        {
            return m_view.RetrieveMediator(mediatorName);
        }

        public IMediator RemoveMediator(string mediatorName)
        {
            return m_view.RemoveMediator(mediatorName);
        }

        public bool HasMediator(string mediatorName)
        {
            return m_view.HasMediator(mediatorName);
        }

        //============================================通知============================================
        public void NotifyObservers(INotification notification)
        {
            m_view.NotifyObservers(notification);
        }


        public override void SendNotification(string notificationName)
        {
            NotifyObservers(new Notification(notificationName));
        }

        public override void SendNotification(string notificationName, object body)
        {
            NotifyObservers(new Notification(notificationName, body));
        }

        public override void SendNotification(string notificationName, object body, string type)
        {
            NotifyObservers(new Notification(notificationName, body, type));
        }

        public static void BroadcastNotification(INotification notification)
        {
            instance.NotifyObservers(notification);
        }

        public static void BroadcastNotification(string notificationName)
        {
            BroadcastNotification(new Notification(notificationName));
        }

        public static void BroadcastNotification(string notificationName, object body)
        {
            BroadcastNotification(new Notification(notificationName, body));
        }

        public static void BroadcastNotification(string notificationName, object body, string type)
        {
            BroadcastNotification(new Notification(notificationName, body, type));
        }

        public void Dispose()
        {
            m_view = null;
            m_model = null;
            m_controller = null;
        }
    }
}
