using System;
using System.Collections.Generic;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class SwitchGroupStateViewModel : ViewModelBase
    {
        private readonly GroupStateContract m_groupState;
        private readonly IDataService m_dataService;
        private readonly IList<GroupInfoViewModel> m_selectedGroups;
        private readonly Action m_refreshAction;
        private bool m_inProgress;
        private bool m_isFlyoutOpen;
        private bool m_showError;

        public SwitchGroupStateViewModel(GroupStateContract groupState, IDataService dataService, IList<GroupInfoViewModel> selectedGroups, Action refreshAction)
        {
            m_groupState = groupState;
            m_dataService = dataService;
            m_selectedGroups = selectedGroups;
            m_refreshAction = refreshAction;

            ChangeStateCommand = new RelayCommand(ChangeGroupState);
        }

        public RelayCommand ChangeStateCommand { get; private set; }

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

        public bool ShowError
        {
            get { return m_showError; }
            set
            {
                m_showError = value;
                RaisePropertyChanged();
            }
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
                        InProgress = false;
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