using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationManager
    {
        private readonly ApplicationLoader m_loader;

        public ApplicationManager()
        {
            m_loader = ApplicationLoader.Instance;
        }

        public ApplicationType CurrentApplication { get; set; }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            var result = m_loader.GetAllApplications();
            callback(result, null);
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            var appInfo = m_loader.GetApplicationByType(type);
            callback(appInfo, null);
        }

        public void GetAllApplicationsByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            var appInfoDictionary = types.ToDictionary(type => type, type => m_loader.GetApplicationByType(type));
            callback(appInfoDictionary, null);
        }

    }
}