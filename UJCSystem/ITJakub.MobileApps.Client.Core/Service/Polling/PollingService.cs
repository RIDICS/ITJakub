using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class PollingService : IPollingService
    {
        private readonly ISynchronizeCommunication m_synchronizeManager;
        private readonly TimerService m_timerService;

        //Mapping from callback for synchronized objects to generic action (for using TimerService)
        private readonly Dictionary<Action<IList<ObjectDetails>, Exception>, Action> m_registeredForSynchronizedObjects;

        public PollingService(TimerService timerService)
        {
            m_timerService = timerService;
            m_synchronizeManager = SynchronizeManager.Instance;
            m_registeredForSynchronizedObjects = new Dictionary<Action<IList<ObjectDetails>, Exception>, Action>();
        }

        public void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType,
            DateTime since, string objectType, Action<IList<ObjectDetails>, Exception> callback)
        {
            var objectParameters = new PollingObjectsParameters
            {
                ApplicationType = applicationType,
                ObjectType = objectType,
                Since = since,
                Callback = callback
            };
            Action action = () => GetSynchronizedObjectsAsync(objectParameters).GetAwaiter().GetResult();
            m_timerService.Register(interval, action);
            m_registeredForSynchronizedObjects.Add(callback, action);
        }

        public void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType, string objectType, Action<IList<ObjectDetails>, Exception> callback)
        {
            RegisterForSynchronizedObjects(interval, applicationType, new DateTime(1970,1,1), objectType, callback);
        }

        public void UnregisterForSynchronizedObjects(PollingInterval interval, Action<IList<ObjectDetails>, Exception> action)
        {
            if (!m_registeredForSynchronizedObjects.ContainsKey(action))
                return;

            var registeredAction = m_registeredForSynchronizedObjects[action];
            m_timerService.Unregister(interval, registeredAction);
            m_registeredForSynchronizedObjects.Remove(action);
        }

        private async Task GetSynchronizedObjectsAsync(PollingObjectsParameters parameters)
        {
            try
            {
                var objects =
                    await
                        m_synchronizeManager.GetObjectsAsync(parameters.ApplicationType, parameters.Since,
                            parameters.ObjectType);
                if (objects.Count == 0)
                    return;

                parameters.Since = objects[objects.Count - 1].CreateTime;
                DispatcherHelper.CheckBeginInvokeOnUI(() => parameters.Callback(objects, null));
            }
            catch (ClientCommunicationException exception)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => parameters.Callback(null, exception));
            }
        }

        private class PollingObjectsParameters
        {
            public ApplicationType ApplicationType { get; set; }
            public string ObjectType { get; set; }
            public DateTime Since { get; set; }
            public Action<IList<ObjectDetails>, Exception> Callback { get; set; }
        }
    }
}