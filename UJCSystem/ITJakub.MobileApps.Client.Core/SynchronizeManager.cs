using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts.Applications;
using Microsoft.Practices.Unity;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core
{
    public class SynchronizeManager : ISynchronizeCommunication
    {
        private static readonly SynchronizeManager m_instance = new SynchronizeManager();
        private readonly AuthenticationManager m_authenticationManager;
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly ApplicationIdManager m_applicationIdManager;
        private readonly GroupManager m_groupManager;

        private SynchronizeManager()
        {
            m_authenticationManager = Container.Current.Resolve<AuthenticationManager>();
            m_serviceClient = Container.Current.Resolve<MobileAppsServiceClient>();
            m_applicationIdManager = Container.Current.Resolve<ApplicationIdManager>();
            m_groupManager = Container.Current.Resolve<GroupManager>();
        }

        public static SynchronizeManager Instance
        {
            get { return m_instance; }
        }

        public async Task SendObjectAsync(ApplicationType applicationType, string objectType, string objectValue)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var synchronizedObject = new SynchronizedObjectContract
            {
                ObjectType = objectType,
                Data = objectValue
            };
            
            var appId = m_applicationIdManager.GetApplicationId(applicationType);
            var groupId = m_groupManager.CurrentGroupId;
            await m_serviceClient.CreateSynchronizedObjectAsync(appId, groupId, userId.Value, synchronizedObject);
        }

        public async Task<IList<ObjectDetails>> GetObjectsAsync(ApplicationType applicationType, DateTime since, string objectType = null)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var appId = m_applicationIdManager.GetApplicationId(applicationType);
            var groupId = m_groupManager.CurrentGroupId;
            var objectList = await m_serviceClient.GetSynchronizedObjectsAsync(groupId, appId, objectType, since);

            var outputList = objectList.Select(objectDetails => new ObjectDetails
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
            return outputList.ToList();
        }

        public async Task<ObjectDetails> GetLatestObjectAsync(ApplicationType applicationType, DateTime since, string objectType)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var appId = m_applicationIdManager.GetApplicationId(applicationType);
            var groupId = m_groupManager.CurrentGroupId;
            var latestObject = await m_serviceClient.GetLatestSynchronizedObjectAsync(groupId, appId, objectType, since);

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

        public IPollingService GetPollingService()
        {
            return Container.Current.Resolve<IPollingService>();
        }

        public async Task CreateTaskAsync(ApplicationType applicationType, string name, string data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var appId = m_applicationIdManager.GetApplicationId(applicationType);
            if (userId != null)
                await m_serviceClient.CreateTaskAsync(userId.Value, appId, name, data);
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