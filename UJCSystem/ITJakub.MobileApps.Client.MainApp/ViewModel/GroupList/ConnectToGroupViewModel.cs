using System;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Error;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class ConnectToGroupViewModel : FlyoutBaseViewModel
    {
        private readonly IDataService m_dataService;
        private readonly Action m_refreshAction;
        private readonly IErrorService m_errorService;
        private string m_connectToGroupCode;
        private bool m_showCodeNotExistError;
        private bool m_showCodeEmptyError;

        public ConnectToGroupViewModel(IDataService dataService, Action refreshAction, IErrorService errorService)
        {
            m_dataService = dataService;
            m_refreshAction = refreshAction;
            m_errorService = errorService;
        }


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

        protected override void SubmitAction()
        {
            ConnectToGroup();
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
                    if (exception is InvalidServerOperationException)
                        ShowCodeNotExistError = true;
                    else
                        m_errorService.ShowConnectionError();
                    return;
                }

                ConnectToGroupCode = string.Empty;
                IsFlyoutOpen = false;
                m_refreshAction();
            });
        }
    }
}