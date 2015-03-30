using System;
using System.Collections.Generic;
using System.Threading;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Error;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class SwitchGroupStateViewModel : FlyoutBaseViewModel
    {
        private readonly GroupStateContract m_groupState;
        private readonly IDataService m_dataService;
        private readonly IList<GroupInfoViewModel> m_selectedGroups;
        private readonly Action m_refreshAction;
        private readonly IErrorService m_errorService;
        private bool m_showError;

        public SwitchGroupStateViewModel(GroupStateContract groupState, IDataService dataService, IList<GroupInfoViewModel> selectedGroups, Action refreshAction, IErrorService errorService)
        {
            m_groupState = groupState;
            m_dataService = dataService;
            m_selectedGroups = selectedGroups;
            m_refreshAction = refreshAction;
            m_errorService = errorService;
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

        protected override void SubmitAction()
        {
            ChangeGroupState();
        }

        private void ChangeGroupState()
        {
            InProgress = true;
            ShowError = false;
            var groupCount = m_selectedGroups.Count;
            
            foreach (var group in m_selectedGroups)
            {
                m_dataService.UpdateGroupState(group.GroupId, m_groupState, exception =>
                {
                    if (exception != null)
                    {
                        if (exception is InvalidServerOperationException)
                            InProgress = false;
                        else
                            m_errorService.ShowConnectionError();

                        ShowError = true;
                    }

                    Interlocked.Decrement(ref groupCount);
                    if (groupCount == 0)
                    {
                        lock (this)
                        {
                            if (groupCount == 0)
                            {
                                InProgress = false;
                                m_refreshAction();
                                groupCount = -1;

                                if (!ShowError)
                                    IsFlyoutOpen = false;
                            }
                        }
                    }
                });
            }
        }
    }
}