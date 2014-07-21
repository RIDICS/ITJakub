using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, object> callback)
        {
            List<ApplicationBase> apps = m_loader.GetAllApplications();
            var result = new ObservableCollection<ApplicationBaseViewModel>();

            foreach (var applicationBase in apps)
            {
                result.Add(applicationBase.ApplicationViewModel);
            }


            callback(result, null);
        }
    }
}