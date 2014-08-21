using System;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public interface ITimerService
    {
        void Register(PollingInterval interval, Action action);
        void Unregister(PollingInterval interval, Action action);
    }
}