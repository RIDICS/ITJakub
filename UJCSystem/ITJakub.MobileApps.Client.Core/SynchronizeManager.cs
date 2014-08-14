using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
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
        private MobileAppsServiceClient m_serviceClient;

        private SynchronizeManager()
        {
            m_authenticationManager = Container.Current.Resolve<AuthenticationManager>();
            m_serviceClient = Container.Current.Resolve<MobileAppsServiceClient>();
        }

        public static SynchronizeManager Instance
        {
            get { return m_instance; }
        }

        //TODO get correct groupId
        private long GroupId { get { return 1; } }

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
            //TODO get correct GroupId and ApplicationId
            await m_serviceClient.CreateSynchronizedObjectAsync(1, GroupId, userId.Value, synchronizedObject);
        }

        public async Task<IEnumerable<ObjectDetails>> GetObjectsAsync(ApplicationType applicationType, DateTime since, string objectType = null)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            var objectList = await m_serviceClient.GetSynchronizedObjectsAsync(GroupId, 1, objectType, since);

            //TODO melo by jit vracet primo object list ale neda mi to informaci jestli to je muj objekt
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
            return outputList;
        }
    }
}