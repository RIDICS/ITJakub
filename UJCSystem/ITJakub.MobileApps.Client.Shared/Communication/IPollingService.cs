using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public interface IPollingService
    {
        void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType,
            DateTime since, string objectType, Action<IList<ObjectDetails>, Exception> callback);

        void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType, string objectType, Action<IList<ObjectDetails>, Exception> callback);
        
        void UnregisterForSynchronizedObjects(PollingInterval interval, Action<IList<ObjectDetails>, Exception> action);
    }
}