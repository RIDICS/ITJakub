using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class EditorHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_applicationName;

        public EditorHostViewModel(INavigationService navigationService, IDataService dataService)
        {
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

        public ViewModelBase EditorViewModel { get; set; }
    }
}