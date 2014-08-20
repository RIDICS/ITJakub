using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class PollingService : IPollingService
    {
        private readonly ISynchronizeCommunication m_synchronizeManager;
        private readonly TimerService m_timerService;

        public PollingService(TimerService timerService)
        {
            m_timerService = timerService;
            m_synchronizeManager = SynchronizeManager.Instance;
        }

        public void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType,
            DateTime since, string objectType, Action<IList<ObjectDetails>> callback)
        {
            var objectsParameters = new PollingObjectsParameters
            {
                ApplicationType = applicationType,
                ObjectType = objectType,
                Since = since,
                Callback = callback
            };
            m_timerService.Register(interval, () => GetSynchronizedObjects(objectsParameters));
        }

        public void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType, string objectType, Action<IList<ObjectDetails>> callback)
        {
            RegisterForSynchronizedObjects(interval, applicationType, new DateTime(1970,1,1), objectType, callback);
        }

        private async void GetSynchronizedObjects(PollingObjectsParameters parameters)
        {
            var objects = await m_synchronizeManager.GetObjectsAsync(parameters.ApplicationType, parameters.Since, parameters.ObjectType);
            if (objects.Count == 0)
                return;

            parameters.Since = objects[objects.Count - 1].CreateTime;
            parameters.Callback(objects);
        }

        private class PollingObjectsParameters
        {
            public ApplicationType ApplicationType { get; set; }
            public string ObjectType { get; set; }
            public DateTime Since { get; set; }
            public Action<IList<ObjectDetails>> Callback { get; set; }
        }
    }
}