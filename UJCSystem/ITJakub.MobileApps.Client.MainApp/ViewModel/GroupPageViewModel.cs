﻿using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
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
        private AppInfoViewModel m_selectedApplicationInfo;
        private GroupInfoViewModel m_groupInfo;
        private TaskViewModel m_selectedTaskViewModel;
        private bool m_changingTask;
        private bool m_taskSaved;
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
            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                LoadData(message.Group);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });

            SelectedApplicationInfo = new AppInfoViewModel();
            SelectedTaskViewModel = new TaskViewModel();
            GroupStates = new ObservableCollection<GroupStateViewModel>();

            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() =>
            {
                m_pollingService.Unregister(MembersPollingInterval, UpdateMembers);
                Messenger.Default.Unregister(this);
                m_navigationService.GoBack();
            });

            SelectAppAndTaskCommand = new RelayCommand(SelectAppAndTask);
            RemoveGroupCommand = new RelayCommand(RemoveGroup);
        }

        private void LoadData(GroupInfoViewModel group)
        {
            GroupInfo = group;

            Loading = true;
            m_dataService.OpenGroupAndGetDetails(group.GroupId, (groupInfo, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                GroupInfo = groupInfo;
                if (groupInfo.Task == null)
                {
                    SelectedTaskViewModel = new TaskViewModel();
                    SelectedApplicationInfo = new AppInfoViewModel {ApplicationType = ApplicationType.Unknown};
                }
                else
                {
                    SelectedTaskViewModel = groupInfo.Task;
                    SelectedApplicationInfo = new AppInfoViewModel {ApplicationType = groupInfo.Task.Application};
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

        public AppInfoViewModel SelectedApplicationInfo
        {
            get { return m_selectedApplicationInfo; }
            set
            {
                m_selectedApplicationInfo = value;
                RaisePropertyChanged();
            }
        }

        public TaskViewModel SelectedTaskViewModel
        {
            get { return m_selectedTaskViewModel; }
            set
            {
                m_selectedTaskViewModel = value;
                RaisePropertyChanged();
            }
        }

        public bool ChangingTask
        {
            get { return m_changingTask; }
            set
            {
                m_changingTask = value;
                ChangingState = m_changingTask;
                RaisePropertyChanged();
            }
        }

        public bool TaskSaved
        {
            get { return m_taskSaved; }
            set
            {
                m_taskSaved = value;
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
        
        private async void SelectAppAndTask()
        {
            ChangingTask = false;
            TaskSaved = false;

            var selectedApp = await ApplicationSelector.SelectApplicationAsync();
            if (selectedApp != null)
            {
                SelectedApplicationInfo = selectedApp;
                SelectTask(selectedApp);
            }
        }

        private void SelectTask(AppInfoViewModel application)
        {
            m_navigationService.Navigate(typeof(SelectTaskView));

            Messenger.Default.Send(new SelectedApplicationMessage {AppInfo = application});

            Messenger.Default.Register<SelectedTaskMessage>(this, message =>
            {
                Messenger.Default.Unregister<SelectTaskView>(this);
                SelectedTaskViewModel = message.TaskInfo;
                SaveTask(message.TaskInfo);
            });
        }

        private void SaveTask(TaskViewModel task)
        {
            TaskSaved = false;
            ChangingTask = true;
            m_dataService.AssignTaskToGroup(m_groupInfo.GroupId, task.Id, exception =>
            {
                ChangingTask = false;
                if (exception != null)
                    return;

                TaskSaved = true;
                GroupInfo.Task = task;
                GroupStateUpdated(GroupInfo.State);
            });
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

                m_navigationService.GoBack();
            });
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