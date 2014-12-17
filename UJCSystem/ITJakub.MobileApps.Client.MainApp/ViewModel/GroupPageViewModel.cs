using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GroupPageViewModel : ViewModelBase
    {
        private const PollingInterval MembersPollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;
        private ApplicationType m_selectedApplication;
        private GroupInfoViewModel m_groupInfo;
        private string m_selectedTaskName;
        private bool m_loading;
        private bool m_changingState;
        private bool m_removing;
        private bool m_isRemoveFlyoutOpen;
        private bool m_savingState;

        /// <summary>
        /// Initializes a new instance of the GroupPageViewModel class.
        /// </summary>
        public GroupPageViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;

            GroupStates = new ObservableCollection<GroupStateViewModel>();
            GroupInfo = new GroupInfoViewModel();

            m_dataService.GetCurrentGroupId(LoadData);

            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack);
            SelectAppAndTaskCommand = new RelayCommand(SelectAppAndTask);
            RemoveGroupCommand = new RelayCommand(RemoveGroup);
        }

        private void LoadData(long groupId)
        {
            Loading = true;
            m_dataService.GetGroupDetails(groupId, (groupInfo, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                GroupInfo = groupInfo;
                if (groupInfo.Task == null)
                {
                    SelectedTaskName = null;
                    SelectedApplication = ApplicationType.Unknown;
                }
                else
                {
                    SelectedTaskName = groupInfo.Task.Name;
                    SelectedApplication = groupInfo.Task.Application;
                }

                GroupStateUpdated(groupInfo.State);
                m_pollingService.RegisterForGroupsUpdate(MembersPollingInterval, new[] {GroupInfo}, UpdateMembers);
            });
        }

        private void GroupStateUpdated(GroupState stateUpdate)
        {
            if (GroupStates.Count == 0)
            {
                for (var i = GroupState.AcceptMembers; i <= GroupState.Closed; i++)
                {
                    GroupStates.Add(new GroupStateViewModel(i, ChangeGroupState));
                }
            }
            foreach (var groupStateViewModel in GroupStates)
            {
                if (GroupInfo.Task == null && groupStateViewModel.GroupState > GroupState.AcceptMembers)
                    groupStateViewModel.IsEnabled = false;
                else
                    groupStateViewModel.IsEnabled = stateUpdate < groupStateViewModel.GroupState || (stateUpdate == GroupState.Paused && groupStateViewModel.GroupState == GroupState.Running);
            }

            GroupInfo.State = stateUpdate;
            RaisePropertyChanged(() => CanChangeTask);
            RaisePropertyChanged(() => CanRemoveGroup);
        }

        private void UpdateMembers(Exception exception)
        {
        }

        public GroupInfoViewModel GroupInfo
        {
            get { return m_groupInfo; }
            set
            {
                m_groupInfo = value;
                RaisePropertyChanged();
            }
        }

        public ApplicationType SelectedApplication
        {
            get { return m_selectedApplication; }
            set
            {
                m_selectedApplication = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedTaskName
        {
            get { return m_selectedTaskName; }
            set
            {
                m_selectedTaskName = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }

        public bool ChangingState
        {
            get { return m_changingState; }
            set
            {
                m_changingState = value;
                RaisePropertyChanged();
            }
        }
        
        public bool Removing
        {
            get { return m_removing; }
            set
            {
                m_removing = value;
                ChangingState = m_removing;
                RaisePropertyChanged();
            }
        }

        public bool SavingState
        {
            get { return m_savingState; }
            set
            {
                m_savingState = value;
                ChangingState = m_savingState;
                RaisePropertyChanged();
            }
        }

        public bool IsRemoveFlyoutOpen
        {
            get { return m_isRemoveFlyoutOpen; }
            set
            {
                m_isRemoveFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool CanChangeTask
        {
            get { return GroupInfo.State < GroupState.Running; }
        }

        public bool CanRemoveGroup
        {
            get { return GroupInfo.State == GroupState.Created || GroupInfo.State == GroupState.Closed; }
        }

        public ObservableCollection<GroupStateViewModel> GroupStates { get; private set; }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand SelectAppAndTaskCommand { get; private set; }

        public RelayCommand RemoveGroupCommand { get; private set; }
        
        private void SelectAppAndTask()
        {
            m_dataService.SetRestoringLastGroupState(true);
            m_dataService.SetAppSelectionTarget(ApplicationSelectionTarget.SelectTask);
            m_navigationService.Navigate<ApplicationSelectionView>();
        }

        private void ChangeGroupState(GroupState newState)
        {
            SavingState = true;
            m_dataService.UpdateGroupState(GroupInfo.GroupId, newState, exception =>
            {
                SavingState = false;
                if (exception != null)
                    return;

                GroupStateUpdated(newState);
            });
        }

        private void RemoveGroup()
        {
            IsRemoveFlyoutOpen = false;

            Removing = true;
            m_dataService.RemoveGroup(GroupInfo.GroupId, exception =>
            {
                Removing = false;
                if (exception != null)
                    return;

                GoBack();
            });
        }

        private void GoBack()
        {
            m_dataService.SetRestoringLastGroupState(false);
            m_dataService.SetAppSelectionTarget(ApplicationSelectionTarget.None);
            m_pollingService.Unregister(MembersPollingInterval, UpdateMembers);
            Messenger.Default.Unregister(this);
            m_navigationService.GoBack();
        }
    }

    public class GroupStateViewModel : ViewModelBase
    {
        private readonly Action<GroupState> m_changeStateAction;
        private bool m_isEnabled;
        private bool m_isFlyoutOpen;

        public GroupStateViewModel(GroupState state, Action<GroupState> changeStateAction)
        {
            m_changeStateAction = changeStateAction;
            GroupState = state;
            ChangeStateCommand = new RelayCommand(ChangeState);
        }

        public GroupState GroupState { get; set; }

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                m_isEnabled = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ChangeStateCommand { get; private set; }

        public bool IsFlyoutOpen
        {
            get { return m_isFlyoutOpen; }
            set
            {
                m_isFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool CanChangeBack
        {
            get { return GroupState == GroupState.Paused; }
        }

        private void ChangeState()
        {
            IsFlyoutOpen = false;
            m_changeStateAction(GroupState);
        }
    }
}