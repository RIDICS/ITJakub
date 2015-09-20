using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationMenuPageViewModel : ViewModelBase
    {
        public IDataService DataService { get; private set; }

        public INavigationService NavigationService { get; private set; }

        public ApplicationMenuPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            DataService = dataService;
            NavigationService = navigationService;
        }
    }
}