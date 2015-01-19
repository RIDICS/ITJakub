using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class ConnectToGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly Action m_refreshAction;
        private string m_connectToGroupCode;
        private bool m_showCodeNotExistError;
        private bool m_inProgress;
        private bool m_isFlyoutOpen;
        private bool m_showCodeEmptyError;

        public ConnectToGroupViewModel(IDataService dataService, Action refreshAction)
        {
            m_dataService = dataService;
            m_refreshAction = refreshAction;

            ConnectToGroupCommand = new RelayCommand(ConnectToGroup);
        }

        public RelayCommand ConnectToGroupCommand { get; private set; }

        public string ConnectToGroupCode
        {
            get { return m_connectToGroupCode; }
            set
            {
                m_connectToGroupCode = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowCodeNotExistError
        {
            get { return m_showCodeNotExistError; }
            set
            {
                m_showCodeNotExistError = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowCodeEmptyError
        {
            get { return m_showCodeEmptyError; }
            set
            {
                m_showCodeEmptyError = value;
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

        public bool IsFlyoutOpen
        {
            get { return m_isFlyoutOpen; }
            set
            {
                m_isFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        private void ConnectToGroup()
        {
            ShowCodeNotExistError = false;
            if (string.IsNullOrWhiteSpace(ConnectToGroupCode))
            {
                ShowCodeEmptyError = true;
                return;
            }

            ShowCodeEmptyError = false;
            InProgress = true;
            m_dataService.ConnectToGroup(ConnectToGroupCode, exception =>
            {
                InProgress = false;
                if (exception != null)
                {
                    ShowCodeNotExistError = true;
                    return;
                }

                ConnectToGroupCode = string.Empty;
                IsFlyoutOpen = false;
                m_refreshAction();
            });
        }
    }
}