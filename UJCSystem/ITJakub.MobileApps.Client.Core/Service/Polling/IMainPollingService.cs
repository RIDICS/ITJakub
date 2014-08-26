using System;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public interface IMainPollingService
    {
        void RegisterForGetTaskByGroup(PollingInterval interval, long groupId, Action<TaskViewModel,Exception> callback);
        void UnregisterForTaskByGroup(PollingInterval interval, Action<TaskViewModel, Exception> action);
    }
}