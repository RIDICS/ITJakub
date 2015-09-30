using System;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class CreateGroupViewModel : FlyoutBaseViewModel
    {
        private readonly IDataService m_dataService;
        private readonly Action m_submitAction;
        private readonly IErrorService m_errorService;
        private string m_newGroupName;
        private bool m_showError;
        private bool m_showNameEmptyError;

        public CreateGroupViewModel(IDataService dataService, IErrorService errorService, Action submitAction = null)
        {
            m_dataService = dataService;
            m_submitAction = submitAction;
            m_errorService = errorService;
        }

        public string NewGroupName
        {
            get { return m_newGroupName; }
            set
            {
                m_newGroupName = value;
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

        protected override void SubmitAction()
        {
            CreateNewGroup();
        }

        protected virtual void CreateNewGroup()
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
                    if (exception is InvalidServerOperationException)
                        ShowError = true;
                    else
                        m_errorService.ShowConnectionError();
                    return;
                }

                m_dataService.SetCurrentGroup(result.GroupId, GroupType.Owner);
                if (m_submitAction != null)
                    m_submitAction();

                NewGroupName = string.Empty;
                IsFlyoutOpen = false;
            });
        }
    }
}
