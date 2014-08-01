using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationManager
    {
        private readonly ApplicationLoader m_loader;

        public ApplicationManager()
        {
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
    }
}