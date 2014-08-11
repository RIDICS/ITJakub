using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.DataService;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class SelectTaskViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;

        public SelectTaskViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
        }
    }
}