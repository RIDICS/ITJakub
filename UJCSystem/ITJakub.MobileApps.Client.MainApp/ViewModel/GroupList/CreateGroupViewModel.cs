using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.MainApp.View;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class CreateGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private string m_newGroupName;
        private bool m_isFlyoutOpen;
        private bool m_inProgress;
        private bool m_showError;
        private bool m_showNameEmptyError;

        public CreateGroupViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            CreateNewGroupCommand = new RelayCommand(CreateNewGroup);
        }

        public RelayCommand CreateNewGroupCommand { get; private set; }

        public string NewGroupName
        {
            get { return m_newGroupName; }
            set
            {
                m_newGroupName = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlyoutOpen
        {
            get { return m_isFlyoutOpen; }
            set
            {
                m_isFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool InProgress
        {
            get { return m_inProgress; }
            set
            {
                m_inProgress = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowError
        {
            get { return m_showError; }
            set
            {
                m_showError = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowNameEmptyError
        {
            get { return m_showNameEmptyError; }
            set
            {
                m_showNameEmptyError = value; 
                RaisePropertyChanged();
            }
        }

        private void CreateNewGroup()
        {
            ShowError = false;
            if (string.IsNullOrWhiteSpace(NewGroupName))
            {
                ShowNameEmptyError = true;
                return;
            }

            ShowNameEmptyError = false;
            InProgress = true;
            m_dataService.CreateNewGroup(NewGroupName, (result, exception) =>
            {
                InProgress = false;
                if (exception != null)
                {
                    ShowError = true;
                    return;
                }
                
                m_dataService.SetCurrentGroup(result.GroupId);
                m_navigationService.Navigate<GroupPageView>();

                NewGroupName = string.Empty;
                IsFlyoutOpen = false;
            });
        }
    }
}
