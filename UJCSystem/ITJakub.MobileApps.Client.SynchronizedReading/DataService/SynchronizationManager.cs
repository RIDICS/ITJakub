using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.SynchronizedReading.DataContract;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataService
{
    public class SynchronizationManager
    {
        private const string UpdateObjectType = "Update";
        private const string ControlObjectType = "PassControl";
        private const PollingInterval SynchronizationPollingInterval = PollingInterval.VeryFast;
        private const PollingInterval ControlPollingInterval = PollingInterval.Medium;

        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly IPollingService m_pollingService;
        private Action<UpdateViewModel, Exception> m_callback;
        private Action<ControlViewModel, Exception> m_controlCallback;
        private ControlContract m_latestControlContract;

        public SynchronizationManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_pollingService = applicationCommunication.GetPollingService();
        }

        public void StartPollingUpdates(Action<UpdateViewModel,Exception> callback)
        {
            m_callback = callback;
            m_pollingService.RegisterForLatestSynchronizedObject(SynchronizationPollingInterval, ApplicationType.SynchronizedReading, UpdateObjectType, ProcessUpdateTypeObject);
        }

        public void StartPollingControlUpdates(Action<ControlViewModel, Exception> callback)
        {
            m_controlCallback = callback;
            m_pollingService.RegisterForLatestSynchronizedObject(ControlPollingInterval, ApplicationType.SynchronizedReading, ControlObjectType, ProcessControlTypeObject);
        }

        public void StopPollingUpdates()
        {
            m_pollingService.UnregisterForLatestSynchronizedObject(SynchronizationPollingInterval, ProcessUpdateTypeObject);
        }

        public void StopPolling()
        {
            StopPollingUpdates();
            m_pollingService.UnregisterForLatestSynchronizedObject(ControlPollingInterval, ProcessControlTypeObject);
        }

        private void ProcessUpdateTypeObject(ObjectDetails objectDetails, Exception exception)
        {
            if (exception != null)
            {
                m_callback(null, exception);
                return;
            }
            
            if (objectDetails == null || objectDetails.Data == null)
            {
                return;
            }

            var updateContract = JsonConvert.DeserializeObject<FullUpdateContract>(objectDetails.Data);
            var updateViewModel = new UpdateViewModel
            {
                SelectionStart = updateContract.SelectionStart,
                SelectionLength = updateContract.SelectionLength,
                CursorPosition = updateContract.CursorPosition,
            };
            if (updateContract.ImageCursorPositionX != null && updateContract.ImageCursorPositionY != null)
            {
                updateViewModel.ContainsImageUpdate = true;
                updateViewModel.ImageCursorPositionX = updateContract.ImageCursorPositionX.Value;
                updateViewModel.ImageCursorPositionY = updateContract.ImageCursorPositionY.Value;
            }

            m_callback(updateViewModel, null);
        }

        private async void ProcessControlTypeObject(ObjectDetails objectDetails, Exception exception)
        {
            if (exception != null)
            {
                m_controlCallback(null, exception);
                return;
            }

            if (objectDetails == null || objectDetails.Data == null)
                return;

            var controlContract = JsonConvert.DeserializeObject<ControlContract>(objectDetails.Data);
            var currentUser = await m_applicationCommunication.GetCurrentUserInfo();
            var controlViewModel = new ControlViewModel
            {
                ReaderUser = MapToUserInfo(controlContract.ReaderUser, currentUser.Id)
            };
            m_latestControlContract = controlContract;

            m_controlCallback(controlViewModel, null);
        }

        private UserInfo MapToUserInfo(ControlContract.UserInfo userInfoContract, long currentUserId)
        {
            return new UserInfo
            {
                FirstName = userInfoContract.FirstName,
                LastName = userInfoContract.LastName,
                Id = userInfoContract.UserId,
                IsMe = userInfoContract.UserId == currentUserId
            };
        }

        private ControlContract.UserInfo MapToUserInfoContract(UserInfo userInfo)
        {
            return new ControlContract.UserInfo
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                UserId = userInfo.Id
            };
        }

        public async void SendUpdate(UpdateViewModel update, Action<Exception> callback)
        {
            try
            {
                UpdateContract updateContract;
                if (update.ContainsImageUpdate)
                {
                    updateContract = new FullUpdateContract
                    {
                        ImageCursorPositionX = update.ImageCursorPositionX,
                        ImageCursorPositionY = update.ImageCursorPositionY
                    };
                }
                else
                {
                    updateContract = new UpdateContract();
                }

                updateContract.SelectionStart = update.SelectionStart;
                updateContract.SelectionLength = update.SelectionLength;
                updateContract.CursorPosition = update.CursorPosition;
                
                var serializedContract = JsonConvert.SerializeObject(updateContract);

                await
                    m_applicationCommunication.SendObjectAsync(ApplicationType.SynchronizedReading, UpdateObjectType,
                        serializedContract);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async void PassControl(UserInfo userInfo, Action<Exception> callback)
        {
            var latestControlContract = m_latestControlContract ?? new ControlContract();
            latestControlContract.ReaderUser = MapToUserInfoContract(userInfo);
            
            try
            {
                var serializedControlContract = JsonConvert.SerializeObject(latestControlContract);
                m_latestControlContract = latestControlContract;

                await m_applicationCommunication.SendObjectAsync(ApplicationType.SynchronizedReading, ControlObjectType, serializedControlContract);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async void TakeReadControl(Action<Exception> callback)
        {
            var userInfo = await m_applicationCommunication.GetCurrentUserInfo();
            PassControl(userInfo, callback);
        }
    }
}