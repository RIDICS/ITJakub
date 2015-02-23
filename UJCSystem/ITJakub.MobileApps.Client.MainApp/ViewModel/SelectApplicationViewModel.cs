using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class SelectApplicationViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private ObservableCollection<IGrouping<ApplicationCategory, AppInfoViewModel>> m_appList;

        public SelectApplicationViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            AppList = new ObservableCollection<IGrouping<ApplicationCategory, AppInfoViewModel>>();
            LoadAppList();

            AppClickCommand = new RelayCommand<ItemClickEventArgs>(AppClick);
            GoBackCommand = new RelayCommand(GoBack);

            m_dataService.GetAppSelectionTarget(async target =>
            {
                if (target == ApplicationSelectionTarget.None)
                {
                    await Task.Delay(50);
                    GoBack();
                }
            });
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
            m_navigationService.GoBack();
        }

        private void AppClick(ItemClickEventArgs args)
        {
            var selectedApp = args.ClickedItem as AppInfoViewModel;
            if (selectedApp == null)
                return;

            m_dataService.SetCurrentApplication(selectedApp.ApplicationType);
            m_dataService.GetAppSelectionTarget(target =>
            {
                switch (target)
                {
                    case ApplicationSelectionTarget.SelectTask:
                        m_navigationService.Navigate<SelectTaskView>();
                        break;
                    case ApplicationSelectionTarget.CreateTask:
                        m_navigationService.Navigate<EditorHostView>();
                        break;
                    default:
                        m_navigationService.GoBack();
                        break;
                }
            });
        }
    }
}