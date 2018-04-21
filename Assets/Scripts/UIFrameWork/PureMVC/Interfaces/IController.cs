using System;
using System.Collections.Generic;

namespace PureMVC.Interfaces
{
    public interface IController : IDisposable
    {
        void RegisterCommand(string notificationName, Type commandType);

        void RegisterCommand(string notificationName, ICommand command);

        void ExecuteCommand(INotification notification);

        object RemoveCommand(string notificationName);

        bool HasCommand(string notificationName);

        IEnumerable<string> ListNotificationNames { get; }
    }
}
