using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class PollingService : IPollingService, IMainPollingService
    {
        private readonly ISynchronizeCommunication m_synchronizeManager;
        private readonly ITimerService m_timerService;
        private readonly GroupManager m_groupManager;
        
        //Mapping from callback to generic action
        private readonly Dictionary<object, Action> m_registeredActions;

        public PollingService(ITimerService timerService, GroupManager groupManager, SynchronizeManager synchronizeManager)
        {
            m_timerService = timerService;
            m_groupManager = groupManager;
            m_synchronizeManager = synchronizeManager;
            m_registeredActions = new Dictionary<object, Action>();
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
            m_registeredActions.Add(callback, action);
        }

        public void RegisterForSynchronizedObjects(PollingInterval interval, ApplicationType applicationType, string objectType, Action<IList<ObjectDetails>, Exception> callback)
        {
            RegisterForSynchronizedObjects(interval, applicationType, new DateTime(1970,1,1), objectType, callback);
        }

        public void RegisterForLatestSynchronizedObject(PollingInterval interval, ApplicationType applicationType, string objectType, Action<ObjectDetails, Exception> callback)
        {
            var objectParameters = new PollingObjectParameters
            {
                ApplicationType = applicationType,
                ObjectType = objectType,
                Since = new DateTime(1970,1,1),
                Callback = callback
            };
            Action action = () => GetLatestSynchronizedObjectAsync(objectParameters).GetAwaiter().GetResult();
            m_timerService.Register(interval, action);
            m_registeredActions.Add(callback, action);
        }

        public void UnregisterForSynchronizedObjects(PollingInterval interval, Action<IList<ObjectDetails>, Exception> action)
        {
            UnregisterGenericAction(interval, action);
        }

        public void UnregisterForLatestSynchronizedObject(PollingInterval interval, Action<ObjectDetails, Exception> action)
        {
            UnregisterGenericAction(interval, action);
        }

        private async Task GetSynchronizedObjectsAsync(PollingObjectsParameters parameters)
        {
            try
            {
                var objects =
                    await
                        m_synchronizeManager.GetObjectsAsync(parameters.ApplicationType, parameters.Since,
                            parameters.ObjectType);

                if (objects.Count > 0)
                    parameters.Since = objects[objects.Count - 1].CreateTime;

                DispatcherHelper.CheckBeginInvokeOnUI(() => parameters.Callback(objects, null));
            }
            catch (ClientCommunicationException exception)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => parameters.Callback(null, exception));
            }
        }

        private async Task GetLatestSynchronizedObjectAsync(PollingObjectParameters parameters)
        {
            try
            {
                var latestObject =
                    await
                        m_synchronizeManager.GetLatestObjectAsync(parameters.ApplicationType, parameters.Since,
                            parameters.ObjectType);

                if (latestObject != null)
                    parameters.Since = latestObject.CreateTime;

                DispatcherHelper.CheckBeginInvokeOnUI(() => parameters.Callback(latestObject, null));
            }
            catch (ClientCommunicationException exception)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => parameters.Callback(null, exception));
            }
        }

        public void RegisterForGetGroupState(PollingInterval interval, long groupId, Action<GroupStateContract, Exception> callback)
        {
            Action newAction = () => m_groupManager.GetGroupStateAsync(groupId, callback).GetAwaiter().GetResult();
            m_timerService.Register(interval, newAction);
            m_registeredActions.Add(callback, newAction);
        }

        public void RegisterForGroupsUpdate(PollingInterval interval, IList<GroupInfoViewModel> groupList, Action<Exception> callback)
        {
            Action newAction = () => m_groupManager.UpdateGroupsMembersAsync(groupList, callback).GetAwaiter().GetResult();
            if (m_registeredActions.ContainsKey(callback))
                return;

            m_timerService.Register(interval, newAction);
            m_registeredActions.Add(callback, newAction);
        }

        private void UnregisterGenericAction(PollingInterval interval, object action)
        {
            if (!m_registeredActions.ContainsKey(action))
                return;

            var registeredAction = m_registeredActions[action];
            m_timerService.Unregister(interval, registeredAction);
            m_registeredActions.Remove(action);
        }

        public void Unregister(PollingInterval interval, Action<GroupStateContract, Exception> action)
        {
            UnregisterGenericAction(interval, action);
        }

        public void Unregister(PollingInterval interval, Action<Exception> action)
        {
            UnregisterGenericAction(interval, action);
        }

        public void UnregisterAll()
        {
            m_timerService.UnregisterAll();
            m_registeredActions.Clear();
        }

        private class PollingObjectsParameters
        {
            public ApplicationType ApplicationType { get; set; }

            public string ObjectType { get; set; }

            public DateTime Since { get; set; }

            public Action<IList<ObjectDetails>, Exception> Callback { get; set; }
        }

        private class PollingObjectParameters
        {
            public ApplicationType ApplicationType { get; set; }

            public string ObjectType { get; set; }

            public DateTime Since { get; set; }

            public Action<ObjectDetails, Exception> Callback { get; set; }
        }
    }
}