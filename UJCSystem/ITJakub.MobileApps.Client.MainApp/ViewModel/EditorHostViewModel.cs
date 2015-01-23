using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class EditorHostViewModel : ViewModelBase
    {
        private readonly INavigationService m_navigationService;
        private readonly IDataService m_dataService;
        private string m_applicationName;

        public EditorHostViewModel(INavigationService navigationService, IDataService dataService)
        {
            m_navigationService = navigationService;
            m_dataService = dataService;
            GoBackCommand = new RelayCommand(navigationService.GoBack);

            m_dataService.GetCurrentApplication(OpenEditor);
        }

        private void OpenEditor(ApplicationType applicationType)
        {
            m_dataService.GetApplication(applicationType, (appInfo, exception) =>
            {
                if (exception != null)
                    return;

                ApplicationName = appInfo.Name;
                EditorViewModel = appInfo.EditorViewModel;
                EditorViewModel.GoBack = () =>
                {
                    m_dataService.SetAppSelectionTarget(ApplicationSelectionTarget.None);
                    m_navigationService.GoBack();
                };
            });
        }

        public RelayCommand GoBackCommand { get; private set; }

        public string ApplicationName
        {
            get { return m_applicationName; }
            set
            {
                m_applicationName = value;
                RaisePropertyChanged();
            }
        }

        public EditorBaseViewModel EditorViewModel { get; set; }
    }
}