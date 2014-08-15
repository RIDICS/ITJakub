using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Manager.Application
{
    public class ApplicationIdManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private Dictionary<string, int> m_applicationIds;

        public ApplicationIdManager(MobileAppsServiceClient serviceClient)
        {
            m_serviceClient = serviceClient;
        }

        private async Task LoadAllApplicationId()
        {
            var appList = await m_serviceClient.GetAllApplication();
            m_applicationIds = new Dictionary<string, int>();
            foreach (var application in appList)
            {
                m_applicationIds[application.Name] = application.Id;
            }
        }

        public async Task<int> GetApplicationIdAsync(ApplicationType applicationType)
        {
            if (m_applicationIds == null)
                await LoadAllApplicationId();

            var applicationName = applicationType.ToString();
            if (m_applicationIds.ContainsKey(applicationName))
                return m_applicationIds[applicationName];

            throw new ArgumentException("Server doesn't know this application.");
        }
    }
}