using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ApplicationSelectionViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private ObservableCollection<IGrouping<ApplicationCategory, AppInfoViewModel>> m_appList;

        /// <summary>
        /// Initializes a new instance of the ApplicationSelectionViewModel class.
        /// </summary>
        public ApplicationSelectionViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            AppList = new ObservableCollection<IGrouping<ApplicationCategory, AppInfoViewModel>>();
            LoadAppList();

            AppClickCommand = new RelayCommand<ItemClickEventArgs>(AppClick);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadAppList()
        {
            m_dataService.GetAllApplications((applications, exception) =>
            {
                var appList = applications.Where(pair => pair.Value.ApplicationRoleType == ApplicationRoleType.MainApp)
                    .Select(applicationKeyValue => new AppInfoViewModel
                {
                    Name = applicationKeyValue.Value.Name,
                    ApplicationType = applicationKeyValue.Key,
                    Icon = applicationKeyValue.Value.Icon,
                    ApplicationCategory = applicationKeyValue.Value.ApplicationCategory
                }).OrderBy(appViewModel => appViewModel.Name)
                    .GroupBy(appViewModel => appViewModel.ApplicationCategory)
                    .OrderBy(appViewModel => appViewModel.Key);

                AppList = new ObservableCollection<IGrouping<ApplicationCategory, AppInfoViewModel>>(appList);
            });
        }

        public ObservableCollection<IGrouping<ApplicationCategory, AppInfoViewModel>> AppList
        {
            get { return m_appList; }
            set
            {
                m_appList = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<ItemClickEventArgs> AppClickCommand { get; private set; }

        public RelayCommand GoBackCommand { get; private set; }

        private void GoBack()
        {
            m_navigationService.GoBackUsingCache();
            MessengerInstance.Send(new SelectedApplicationMessage());
        }

        private void AppClick(ItemClickEventArgs args)
        {
            var selectedApp = args.ClickedItem as AppInfoViewModel;
            if (selectedApp == null)
                return;

            m_navigationService.GoBackUsingCache();
            Messenger.Default.Send(new SelectedApplicationMessage
            {
                AppInfo = selectedApp
            });
        }
    }
}