using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Communication.Client;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts.Applications;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class SynchronizeManager : ISynchronizeCommunication
    {
        private const int SyncObjectRequestCount = 100;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly MobileAppsServiceClientManager m_serviceClient;
        private readonly ApplicationIdManager m_applicationIdManager;
        private readonly GroupManager m_groupManager;

        public SynchronizeManager(AuthenticationManager authenticationManager, MobileAppsServiceClientManager serviceClient,
            ApplicationIdManager applicationIdManager, GroupManager groupManager)
        {
            m_authenticationManager = authenticationManager;
            m_serviceClient = serviceClient;
            m_applicationIdManager = applicationIdManager;
            m_groupManager = groupManager;
        }

        private SynchronizationTypeContract ConvertSynchronizationType(SynchronizationType synchronizationType)
        {
            switch (synchronizationType)
            {
                case SynchronizationType.HistoryTrackingObject:
                    return SynchronizationTypeContract.HistoryTrackingObject;
                case SynchronizationType.SingleObject:
                    return SynchronizationTypeContract.SingleObject;
            }
            return SynchronizationTypeContract.HistoryTrackingObject;
        }

        public async Task SendObjectAsync(ApplicationType applicationType, string objectType, string objectValue, SynchronizationType synchronizationType)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var synchronizedObject = new SynchronizedObjectContract
            {
                ObjectType = objectType,
                Data = objectValue,
                SynchronizationType = ConvertSynchronizationType(synchronizationType)
            };

            var appId = await m_applicationIdManager.GetApplicationId(applicationType);
            var groupId = m_groupManager.CurrentGroupId;

            var client = m_serviceClient.GetClient();
            await client.CreateSynchronizedObjectAsync(appId, groupId, userId.Value, synchronizedObject);
        }

        public async Task<IList<ObjectDetails>> GetObjectsAsync(ApplicationType applicationType, DateTime since, string objectType = null)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var appId = await m_applicationIdManager.GetApplicationId(applicationType);
            var groupId = m_groupManager.CurrentGroupId;
            var client = m_serviceClient.GetClient();

            IList<SynchronizedObjectResponseContract> objectList;
            var outputList = new List<ObjectDetails>();

            do
            {
                objectList = await client.GetSynchronizedObjectsAsync(groupId, appId, objectType, since, SyncObjectRequestCount);

                var tempOutputList = objectList.Select(objectDetails => new ObjectDetails
                {
                    Author = new UserInfo
                    {
                        Email = objectDetails.Author.Email,
                        FirstName = objectDetails.Author.FirstName,
                        LastName = objectDetails.Author.LastName,
                        Id = objectDetails.Author.Id,
                        IsMe = (userId == objectDetails.Author.Id)
                    },
                    CreateTime = objectDetails.CreateTime,
                    Data = objectDetails.Data
                });

                outputList.AddRange(tempOutputList);
                var latestItem = outputList.LastOrDefault();
                if (latestItem != null)
                {
                    since = latestItem.CreateTime;
                }

            } while (objectList.Count == SyncObjectRequestCount);

            return outputList;
        }

        public async Task<ObjectDetails> GetLatestObjectAsync(ApplicationType applicationType, DateTime since, string objectType)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var appId = await m_applicationIdManager.GetApplicationId(applicationType);
            var groupId = m_groupManager.CurrentGroupId;
            var client = m_serviceClient.GetClient();
            var latestObject = await client.GetLatestSynchronizedObjectAsync(groupId, appId, objectType, since);

            if (latestObject == null)
                return null;

            var objectDetails = new ObjectDetails
            {
                Author = new UserInfo
                {
                    Email = latestObject.Author.Email,
                    FirstName = latestObject.Author.FirstName,
                    LastName = latestObject.Author.LastName,
                    Id = latestObject.Author.Id,
                    IsMe = (userId.Value == latestObject.Author.Id)
                },
                CreateTime = latestObject.CreateTime,
                Data = latestObject.Data
            };
            return objectDetails;
        }

        public IPollingService PollingService
        {
            get { return Container.Current.Resolve<IPollingService>(); }
        }

        public IErrorService ErrorService
        {
            get { return Container.Current.Resolve<IErrorService>(); }
        }

        public async Task CreateTaskAsync(ApplicationType applicationType, string name, string description, string data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var appId = await m_applicationIdManager.GetApplicationId(applicationType);
            if (userId != null)
            {
                var client = m_serviceClient.GetClient();
                await client.CreateTaskAsync(userId.Value, appId, name, description, data);
            }
        }

        public Task<UserInfo> GetCurrentUserInfo()
        {
            var taskCompletition = new TaskCompletionSource<UserInfo>();
            m_authenticationManager.GetLoggedUserInfo(false, userViewModel =>
            {
                var userInfo = new UserInfo
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    Id = userViewModel.UserId,
                    IsMe = true
                };
                taskCompletition.SetResult(userInfo);
            });
            return taskCompletition.Task;
        }
    }
}