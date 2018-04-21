using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

namespace PureMVC.Core
{
    public class Model : IModel
    {
        private static IModel instance = null;
        public static IModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new Model();
                return instance;
            }
        }

        protected string m_multitonKey;
        protected IDictionary<string, IProxy> m_proxyMap;
        public const string DEFAULT_KEY = "PureMVC";


        protected Model()
        {
            m_multitonKey = DEFAULT_KEY;
            m_proxyMap = new Dictionary<string, IProxy>();
            InitializeModel();
        }

        protected virtual void InitializeModel()
        {

        }


        public virtual void RegisterProxy(IProxy proxy)
        {
            proxy.InitializeNotifier(m_multitonKey);
            m_proxyMap[proxy.ProxyName] = proxy;

            proxy.OnRegister();
        }

        public virtual IProxy RetrieveProxy(string proxyName)
        {
            if (!m_proxyMap.ContainsKey(proxyName)) return null;

            return m_proxyMap[proxyName];
        }

        public virtual bool HasProxy(string proxyName)
        {
            return m_proxyMap.ContainsKey(proxyName);
        }

        public IEnumerable<string> ListProxyNames
        {
            get { return m_proxyMap.Keys; }
        }

        public virtual IProxy RemoveProxy(string proxyName)
        {
            IProxy proxy = null;

            if (m_proxyMap.ContainsKey(proxyName))
            {
                proxy = RetrieveProxy(proxyName);
                m_proxyMap.Remove(proxyName);
            }

            if (proxy != null) proxy.OnRemove();
            return proxy;
        }

        

        public void Dispose()
        {
            m_proxyMap.Clear();
        }
    }
}
