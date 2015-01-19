using System;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class CreateGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly Action m_refreshAction;
        private string m_newGroupName;
        private bool m_isFlyoutOpen;
        private bool m_inProgress;
        private bool m_showError;
        private bool m_showNameEmptyError;

        public CreateGroupViewModel(IDataService dataService, Action refreshAction)
        {
            m_dataService = dataService;
            m_refreshAction = refreshAction;

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
                
                //TODO navigate to groupPage
                new MessageDialog(result.EnterCode, "Nová skupina vytvořena").ShowAsync();
                NewGroupName = string.Empty;
                IsFlyoutOpen = false;
                m_refreshAction();
            });
        }
    }
}
