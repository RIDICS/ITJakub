using System;
using System.Collections.Generic;
using System.Threading;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Error;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class DeleteGroupViewModel : FlyoutBaseViewModel
    {
        private readonly IDataService m_dataService;
        private readonly List<GroupInfoViewModel> m_selectedGroups;
        private readonly Action m_refreshAction;
        private readonly IErrorService m_errorService;
        private bool m_showError;
        private GroupInfoViewModel m_selectedGroup;
        private int m_selectedGroupCount;

        public DeleteGroupViewModel(IDataService dataService, List<GroupInfoViewModel> selectedGroups, Action refreshAction, IErrorService errorService)
        {
            m_dataService = dataService;
            m_selectedGroups = selectedGroups;
            m_refreshAction = refreshAction;
            m_errorService = errorService;
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

        public int SelectedGroupCount
        {
            get { return m_selectedGroupCount; }
            set
            {
                m_selectedGroupCount = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => IsOneItemSelected);
                RaisePropertyChanged(() => IsManyItemsSelected);
            }
        }

        public bool IsOneItemSelected
        {
            get { return SelectedGroupCount == 1; }
        }

        public bool IsManyItemsSelected
        {
            get { return SelectedGroupCount > 1; }
        }
        
        protected override void SubmitAction()
        {
            DeleteGroup();
        }

        private void DeleteGroup()
        {
            ShowError = false;
            InProgress = true;
            var groupCount = m_selectedGroups.Count;

            foreach (var @group in m_selectedGroups)
            {
                m_dataService.RemoveGroup(group.GroupId, exception =>
                {
                    if (exception != null)
                    {
                        if (exception is InvalidServerOperationException)
                            ShowError = true;
                        else
                            m_errorService.ShowConnectionError();

                        InProgress = false;
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
