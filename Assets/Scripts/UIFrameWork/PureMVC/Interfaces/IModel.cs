using System;
using System.Collections.Generic;

namespace PureMVC.Interfaces
{
    public interface IModel : IDisposable
    {
        void RegisterProxy(IProxy proxy);

        IProxy RetrieveProxy(string proxyName);

        IProxy RemoveProxy(string proxyName);

        bool HasProxy(string proxyName);

        IEnumerable<string> ListProxyNames { get; }
    }
}
