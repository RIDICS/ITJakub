using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Microsoft.Practices.Unity;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core
{
    public class SynchronizeManager : ISynchronizeCommunication
    {
        private static readonly SynchronizeManager m_instance = new SynchronizeManager();
        private readonly AuthenticationManager m_authenticationManager;
        private readonly MobileAppsServiceManager m_serviceManager;

        private SynchronizeManager()
        {
            m_authenticationManager = Container.Current.Resolve<AuthenticationManager>();
            m_serviceManager = Container.Current.Resolve<MobileAppsServiceManager>();
        }

        public static SynchronizeManager Instance
        {
            get { return m_instance; }
        }

        //TODO get correct groupId
        private long GroupId { get { return 1; } }

        public Task SendObjectAsync(ApplicationType applicationType, string objectType, string objectValue)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            return m_serviceManager.SendSynchronizedObjectAsync(applicationType, GroupId, userId.Value, objectType, objectValue);
        }

        public Task<IEnumerable<ObjectDetails>> GetObjectsAsync(ApplicationType applicationType, DateTime since, string objectType = null)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            if (!userId.HasValue)
                throw new ArgumentException("No logged user");

            return m_serviceManager.GetSynchronizedObjectsAsync(applicationType, GroupId, objectType, since);
        }
    }
}