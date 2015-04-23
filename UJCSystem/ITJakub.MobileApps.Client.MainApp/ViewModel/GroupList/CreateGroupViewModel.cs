using System;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class CreateGroupViewModel : FlyoutBaseViewModel
    {
        private readonly IDataService m_dataService;
        private readonly Action<Type> m_navigationAction;
        private readonly IErrorService m_errorService;
        private string m_newGroupName;
        private bool m_showError;
        private bool m_showNameEmptyError;

        public CreateGroupViewModel(IDataService dataService, Action<Type> navigationAction, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationAction = navigationAction;
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
                    if (exception is InvalidServerOperationException)
                        ShowError = true;
                    else
                        m_errorService.ShowConnectionError();
                    return;
                }
                
                m_dataService.SetCurrentGroup(result.GroupId);
                m_navigationAction(typeof(GroupPageView));

                NewGroupName = string.Empty;
                IsFlyoutOpen = false;
            });
        }
    }
}
