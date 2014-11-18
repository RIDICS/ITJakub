using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;

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

            MessengerInstance.Register<OpenEditorMessage>(this, OpenEditor);
        }

        private void OpenEditor(OpenEditorMessage message)
        {
            m_dataService.GetApplication(message.Application, (appInfo, exception) =>
            {
                if (exception != null)
                    return;

                ApplicationName = appInfo.Name;
                EditorViewModel = appInfo.EditorViewModel;
            });
            MessengerInstance.Unregister<OpenEditorMessage>(this);
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