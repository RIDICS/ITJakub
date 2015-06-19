using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public interface ISynchronizeCommunication
    {
        Task SendObjectAsync(ApplicationType applicationType, string objectType, string objectValue, SynchronizationType synchronizationType = SynchronizationType.HistoryTrackingObject);

        Task<IList<ObjectDetails>> GetObjectsAsync(ApplicationType applicationType, DateTime since, string objectType = null);

        Task<ObjectDetails> GetLatestObjectAsync(ApplicationType applicationType, DateTime since, string objectType);

        IPollingService PollingService { get; }

        IErrorService ErrorService { get; }

        Task CreateTaskAsync(ApplicationType applicationType, string name, string data);

        Task<UserInfo> GetCurrentUserInfo();
    }
}