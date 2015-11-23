using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class DuplicateGroupViewModel : FlyoutBaseViewModel
    {
        private readonly IDataService m_dataService;
        private readonly IErrorService m_errorService;
        private readonly List<GroupInfoViewModel> m_selectedGroups;
        private readonly Action m_submitAction;        
        private string m_newGroupName;
        private GroupInfoViewModel m_selectedGroup;
        private bool m_showError;
        private bool m_showNameEmptyError;


        public DuplicateGroupViewModel(IDataService dataService, IErrorService errorService, List<GroupInfoViewModel> selectedGroups,
            Action submitAction = null)
        {
            m_dataService = dataService;
            m_errorService = errorService;
            m_selectedGroups = selectedGroups;
            m_submitAction = submitAction;
        }


        public GroupInfoViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                m_selectedGroup = value;
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
     

        public string NewGroupName
        {
            get { return m_newGroupName; }
            set
            {
                m_newGroupName = value;
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
          

            ShowError = false;
            if (string.IsNullOrWhiteSpace(NewGroupName))
            {
                ShowNameEmptyError = true;
                return;
            }

            if (m_selectedGroups.Count != 1)
            {
                ShowError = true;
                return;
            }

            ShowNameEmptyError = false;
            InProgress = true;

            var selectedGroupId = m_selectedGroups.First().GroupId;

            m_dataService.DuplicateGroup(selectedGroupId, NewGroupName, (result, exception) =>
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
                    m_submitAction.Invoke();

                NewGroupName = string.Empty;
                IsFlyoutOpen = false;
            });
        }
    }
}