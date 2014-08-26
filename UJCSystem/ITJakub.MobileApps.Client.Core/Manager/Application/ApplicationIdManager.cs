using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Manager.Application
{
    public class ApplicationIdManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private Dictionary<ApplicationType, int> m_applicationTypeToId;
        private Dictionary<int, ApplicationType> m_applicaitonIdToType; 

        public ApplicationIdManager(MobileAppsServiceClient serviceClient)
        {
            m_serviceClient = serviceClient;
        }

        private void LoadAllApplicationId()
        {
            var appList = m_serviceClient.GetAllApplication().Result;
            
            m_applicationTypeToId = new Dictionary<ApplicationType, int>();
            m_applicaitonIdToType = new Dictionary<int, ApplicationType>();

            foreach (var application in appList)
            {
                var applicationType = ConvertStringToAppType(application.Name);
                if (applicationType == ApplicationType.Unknown)
                    continue;

                m_applicationTypeToId[applicationType] = application.Id;
                m_applicaitonIdToType[application.Id] = applicationType;
            }
        }

        private ApplicationType ConvertStringToAppType(string applicationName)
        {
            ApplicationType appType;
            return Enum.TryParse(applicationName, true, out appType) ? appType : ApplicationType.Unknown;
        }

        public int GetApplicationId(ApplicationType applicationType)
        {
            if (m_applicationTypeToId == null)
                LoadAllApplicationId();

            if (m_applicationTypeToId.ContainsKey(applicationType))
                return m_applicationTypeToId[applicationType];

            throw new ArgumentException("Server doesn't know this application type.");
        }

        public ApplicationType GetApplicationType(int applicationId)
        {
            if (m_applicaitonIdToType == null)
                LoadAllApplicationId();

            if (m_applicaitonIdToType.ContainsKey(applicationId))
                return m_applicaitonIdToType[applicationId];

            throw new ArgumentException("Server doesn't know this application ID.");
        }
    }
}