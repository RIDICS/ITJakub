using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public interface IMainPollingService
    {
        void RegisterForGetTaskByGroup(PollingInterval interval, long groupId, Action<TaskViewModel,Exception> callback);
        void Unregister(PollingInterval interval, Action<TaskViewModel, Exception> action);

        void RegisterForGroupsUpdate(PollingInterval interval, IList<GroupInfoViewModel> groupList, Action<Exception> callback);
        void Unregister(PollingInterval interval, Action<Exception> action);

        void UnregisterAll();
    }
}