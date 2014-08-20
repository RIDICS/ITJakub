using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public interface IPollingService
    {
        void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType,
            DateTime since, string objectType, Action<IList<ObjectDetails>> callback);

        void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType, string objectType, Action<IList<ObjectDetails>> callback);
    }
}