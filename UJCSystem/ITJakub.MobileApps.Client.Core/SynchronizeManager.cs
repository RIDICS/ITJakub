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
                Author = new AuthorInfo
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

        public IPollingService GetPollingService()
        {
            return Container.Current.Resolve<IPollingService>();
        }
    }
}