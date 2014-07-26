using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;

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
        private ObservableCollection<AppInfoViewModel> m_appList;
        private RelayCommand<ItemClickEventArgs> m_appClickCommand;

        /// <summary>
        /// Initializes a new instance of the ApplicationSelectionViewModel class.
        /// </summary>
        public ApplicationSelectionViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            AppList = new ObservableCollection<AppInfoViewModel>();
            LoadAppList();
            m_appClickCommand = new RelayCommand<ItemClickEventArgs>(AppClick);
        }

        private void LoadAppList()
        {
            m_dataService.GetAllApplications((applications, exception) =>
            {
                AppList.Clear();
                foreach (var applicationKeyValue in applications)
                {
                    AppList.Add(new AppInfoViewModel
                    {
                        Name = applicationKeyValue.Value.Name,
                        ApplicationType = applicationKeyValue.Key,
                        Icon = applicationKeyValue.Value.Icon
                    });
                }
            });
        }

        public ObservableCollection<AppInfoViewModel> AppList
        {
            get { return m_appList; }
            set
            {
                m_appList = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<ItemClickEventArgs> AppClickCommand
        {
            get { return m_appClickCommand; }
        }

        private void AppClick(ItemClickEventArgs args)
        {
            var selectedApp = args.ClickedItem as AppInfoViewModel;
            if (selectedApp == null)
                return;
            m_navigationService.Navigate(typeof (ApplicationHostView), selectedApp.ApplicationType);
        }
    }
}