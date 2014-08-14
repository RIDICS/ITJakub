using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly ApplicationLoader m_loader;
        private Dictionary<string, int> m_applicationIds;

        public ApplicationManager(MobileAppsServiceClient serviceClient)
        {
            m_serviceClient = serviceClient;
            m_loader = ApplicationLoader.Instance;
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
        {
            var apps = m_loader.GetAllApplications();
            var result = new ObservableCollection<ApplicationBaseViewModel>();

            foreach (var applicationBase in apps)
            {
                result.Add(applicationBase.Value.ApplicationViewModel);
            }

            callback(result, null);
        }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            var result = m_loader.GetAllApplications();
            callback(result, null);
        }

        public ApplicationBase GetApplication(ApplicationType type)
        {
            return m_loader.GetApplicationByType(type);
        }

        public Dictionary<ApplicationType, ApplicationBase> GetAllApplicationsByTypes(IEnumerable<ApplicationType> types)
        {
            return types.ToDictionary(type => type, GetApplication);
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

        public async Task<int> GetApplicationId(ApplicationType applicationType)
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