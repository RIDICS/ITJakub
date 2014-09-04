using System;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public interface IPollingTimer
    {
        void Register(Action action);
        void Unregister(Action action);
        void UnregisterAll();
    }
}