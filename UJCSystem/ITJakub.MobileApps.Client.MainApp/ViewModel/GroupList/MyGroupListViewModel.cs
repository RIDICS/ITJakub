using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class MyGroupListViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;

        public MyGroupListViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
        }

        public RelayCommand GoBackCommand { get; private set; }
    }
}