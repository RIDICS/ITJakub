using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupPage
{
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
        private bool m_canConnectToGroup;
        private long m_currentUserId;
        private bool m_connectingToGroup;

        public GroupPageViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;

            GroupStates = new ObservableCollection<GroupStateViewModel>();
            GroupInfo = new GroupInfoViewModel();

            m_dataService.GetCurrentGroupId(LoadData);
            m_dataService.GetLoggedUserInfo(false, userInfo =>
            {
                m_currentUserId = userInfo.UserId;
            });

            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack);
            SelectAppAndTaskCommand = new RelayCommand(SelectAppAndTask);
            RemoveGroupCommand = new RelayCommand(RemoveGroup);
            ConnectToGroupCommand = new RelayCommand(ConnectToGroup);
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
                { //TODO nastane tahle vetev vubec nekdy??
                    SelectedTaskName = groupInfo.Task.Name;
                    SelectedApplication = groupInfo.Task.Application;
                }

                GroupStateUpdated(groupInfo.State);
                m_pollingService.RegisterForGroupsUpdate(MembersPollingInterval, new[] {GroupInfo}, UpdateMembers);
            });
        }

        private void GroupStateUpdated(GroupStateContract stateUpdate)
        {
            if (GroupStates.Count == 0)
            {
                for (var i = GroupStateContract.AcceptMembers; i <= GroupStateContract.Closed; i++)
                {
                    GroupStates.Add(new GroupStateViewModel(i, ChangeGroupState));
                }
            }
            foreach (var groupStateViewModel in GroupStates)
            {
                if (GroupInfo.Task == null && groupStateViewModel.GroupState > GroupStateContract.AcceptMembers)
                    groupStateViewModel.IsEnabled = false;
                else
                    groupStateViewModel.IsEnabled = stateUpdate < groupStateViewModel.GroupState || (stateUpdate == GroupStateContract.Paused && groupStateViewModel.GroupState == GroupStateContract.Running);
            }

            GroupInfo.State = stateUpdate;
            RaisePropertyChanged(() => CanChangeTask);
            RaisePropertyChanged(() => CanRemoveGroup);
            UpdateCanConnectToGroup();
        }

        private void UpdateMembers(Exception exception)
        {
            UpdateCanConnectToGroup();
        }

        private void UpdateCanConnectToGroup()
        {
            CanConnectToGroup = GroupInfo.State >= GroupStateContract.AcceptMembers && !GroupInfo.ContainsMember(m_currentUserId);
        }

        public GroupInfoViewModel GroupInfo
        {
            get { return m_groupInfo; }
            set
            {
                m_groupInfo = value;
                RaisePropertyChanged();
                UpdateCanConnectToGroup();
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

        public bool ConnectingToGroup
        {
            get { return m_connectingToGroup; }
            set
            {
                m_connectingToGroup = value;
                RaisePropertyChanged();
            }
        }

        public bool CanConnectToGroup
        {
            get { return m_canConnectToGroup; }
            set
            {
                m_canConnectToGroup = value;
                RaisePropertyChanged();
            }
        }

        public bool CanChangeTask
        {
            get { return GroupInfo.State < GroupStateContract.Running; }
        }

        public bool CanRemoveGroup
        {
            get { return GroupInfo.State == GroupStateContract.Created || GroupInfo.State == GroupStateContract.Closed; }
        }

        public ObservableCollection<GroupStateViewModel> GroupStates { get; private set; }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand SelectAppAndTaskCommand { get; private set; }

        public RelayCommand RemoveGroupCommand { get; private set; }

        public RelayCommand ConnectToGroupCommand { get; private set; }

        private void SelectAppAndTask()
        {
            m_dataService.SetRestoringLastGroupState(true);
            m_dataService.SetAppSelectionTarget(ApplicationSelectionTarget.SelectTask);
            Navigate<ApplicationSelectionView>();
        }

        private void ChangeGroupState(GroupStateContract newState)
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

        private void Navigate<T>()
        {
            m_pollingService.Unregister(MembersPollingInterval, UpdateMembers);
            m_navigationService.Navigate<T>();
        }

        private void GoBack()
        {
            m_dataService.SetRestoringLastGroupState(false);
            m_dataService.SetAppSelectionTarget(ApplicationSelectionTarget.None);
            m_pollingService.Unregister(MembersPollingInterval, UpdateMembers);
            m_navigationService.GoBack();
        }

        private void ConnectToGroup()
        {
            CanConnectToGroup = false;
            ConnectingToGroup = true;
            m_dataService.ConnectToGroup(GroupInfo.GroupCode, exception =>
            {
                ConnectingToGroup = false;

                if (exception != null)
                {
                    CanConnectToGroup = true;
                }
            });
        }
    }
}