using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public interface IMainPollingService
    {
        void RegisterForGetGroupState(PollingInterval interval, long groupId, Action<GroupStateContract, Exception> callback);

        void RegisterForGroupsUpdate(PollingInterval interval, IList<GroupInfoViewModel> groupList, Action<Exception> callback);

        void Unregister(PollingInterval interval, Action<GroupStateContract, Exception> action);
        

        void Unregister(PollingInterval interval, Action<Exception> action);

        void UnregisterAll();
    }
}