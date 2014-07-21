using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_applicationName;
        private ObservableCollection<ApplicationBaseViewModel> m_allApps;


        public ApplicationHostViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            LoadInitData();
        }

        private void LoadInitData()
        {
            m_dataService.GetAllApplicationViewModels((data, exception) =>
            {
                if (exception != null)
                    return;
                else
                    m_allApps = data;
            });
        }

        public string ApplicationName
        {
            get { return m_applicationName; }
            set
            {
                m_applicationName = value;
                RaisePropertyChanged();
            }
        }

        public ApplicationBaseViewModel ApplicationViewModel { get; set; }

        public bool IsChatSupported { get; protected set; }
    }
}
