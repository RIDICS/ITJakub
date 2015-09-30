using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Communication.Error;
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
        private readonly IErrorService m_errorService;
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
        private BitmapImage m_appIcon;
        private string m_appName;
        private bool m_canOpenApplication;
        private bool m_isGlobalFocus;
        private long m_groupId;
        private bool m_canOpenAdmin;
        private bool m_isTaskSelected;
        private bool m_showRenewError;
        private bool m_renewCodeInProgress;

        public GroupPageViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService,
            IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;
            m_errorService = errorService;

            GroupStates = new ObservableCollection<GroupStateViewModel>();
            GroupRemoveViewModel = new GroupRemoveViewModel(RemoveGroup);
            GroupInfo = new GroupInfoViewModel();

            m_dataService.GetCurrentGroupId((groupId, groupType) =>
            {
                m_groupId = groupId;
                LoadData();
            });
            m_dataService.GetLoggedUserInfo(false, userInfo => { m_currentUserId = userInfo.UserId; });

            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack);
            SelectAppAndTaskCommand = new RelayCommand(SelectAppAndTask);
            ConnectToGroupCommand = new RelayCommand(ConnectToGroup);
            OpenApplicationCommand = new RelayCommand(Navigate<ApplicationHostView>);
            OpenAdminCommand = new RelayCommand(OpenAdminView);
            ReloadCommand = new RelayCommand(LoadData);
            ShowTaskCommand = new RelayCommand(ShowTask);
            ReNewCodeForGroup = new RelayCommand(RenewInputCode);
        }

        private void LoadData()
        {
            Loading = true;
            m_pollingService.Unregister(MembersPollingInterval, UpdateMembers);
            m_dataService.GetGroupDetails(m_groupId, (groupInfo, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionError(GoBack);
                    return;
                }

                GroupInfo = groupInfo;
                if (groupInfo.Task == null)
                {
                    SelectedTaskName = null;
                    LoadAppInfo(ApplicationType.Unknown);
                }
                else
                {
                    SelectedTaskName = groupInfo.Task.Name;
                    LoadAppInfo(groupInfo.Task.Application);
                }

                GroupStateUpdated(groupInfo.State);
                m_pollingService.RegisterForGroupsUpdate(MembersPollingInterval, new[] {GroupInfo}, UpdateMembers);
            });
        }

        private void LoadAppInfo(ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.Unknown)
            {
                AppName = null;
                AppIcon = null;
                return;
            }

            m_dataService.GetApplication(applicationType, (appInfo, exception) =>
            {
                if (exception != null)
                {
                    AppName = "(Neznámá aplikace)";
                    AppIcon = null;
                    return;
                }

                AppIcon = appInfo.Icon;
                AppName = appInfo.Name;
            });
        }

        private void GroupStateUpdated(GroupStateContract stateUpdate)
        {
            if (GroupStates.Count == 0)
            {
                for (var i = GroupStateContract.AcceptMembers; i <= GroupStateContract.Closed; i++)
                {
                    if (i == GroupStateContract.WaitingForStart) // this state is disabled
                        continue;

                    GroupStates.Add(new GroupStateViewModel(i, ChangeGroupState));
                }
            }
            foreach (var groupStateViewModel in GroupStates)
            {
                if (GroupInfo.Task == null && groupStateViewModel.GroupState > GroupStateContract.AcceptMembers)
                    groupStateViewModel.IsEnabled = false;
                else
                    groupStateViewModel.IsEnabled = stateUpdate < groupStateViewModel.GroupState ||
                                                    (stateUpdate == GroupStateContract.Paused && groupStateViewModel.GroupState == GroupStateContract.Running);

                groupStateViewModel.IsCurrentState = groupStateViewModel.GroupState == stateUpdate;
            }

            GroupInfo.State = stateUpdate;
            RaisePropertyChanged(() => CanChangeTask);
            RaisePropertyChanged(() => CanRemoveGroup);
            UpdateCanConnectToGroup();
        }

        private void UpdateMembers(Exception exception)
        {
            if (exception != null)
                m_errorService.ShowConnectionWarning();
            else
                DispatcherHelper.CheckBeginInvokeOnUI((() =>
                {
                    m_errorService.HideWarning();
                    UpdateCanConnectToGroup();
                }));
        }

        private void UpdateCanConnectToGroup()
        {
            CanOpenAdmin = GroupInfo.State >= GroupStateContract.Running;
            CanConnectToGroup = GroupInfo.State >= GroupStateContract.AcceptMembers && !GroupInfo.ContainsMember(m_currentUserId);
            CanOpenApplication = GroupInfo.State >= GroupStateContract.Running && GroupInfo.ContainsMember(m_currentUserId);
        }

        public GroupInfoViewModel GroupInfo
        {
            get { return m_groupInfo; }
            set
            {
                m_groupInfo = value;
                RaisePropertyChanged();
                UpdateCanConnectToGroup();
                IsTaskSelected = m_groupInfo.Task != null;
            }
        }

        public BitmapImage AppIcon
        {
            get { return m_appIcon; }
            set
            {
                m_appIcon = value;
                RaisePropertyChanged();
            }
        }

        public string AppName
        {
            get { return m_appName; }
            set
            {
                m_appName = value;
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

        public bool CanOpenApplication
        {
            get { return m_canOpenApplication; }
            set
            {
                m_canOpenApplication = value;
                RaisePropertyChanged();
            }
        }

        public bool CanOpenAdmin
        {
            get { return m_canOpenAdmin; }
            set
            {
                m_canOpenAdmin = value;
                RaisePropertyChanged();
            }
        }

        public bool IsGlobalFocus
        {
            get { return m_isGlobalFocus; }
            set
            {
                m_isGlobalFocus = value;
                RaisePropertyChanged();
            }
        }

        public bool IsTaskSelected
        {
            get { return m_isTaskSelected; }
            set
            {
                m_isTaskSelected = value;
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

        public GroupRemoveViewModel GroupRemoveViewModel { get; private set; }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand SelectAppAndTaskCommand { get; private set; }

        public RelayCommand ConnectToGroupCommand { get; private set; }

        public RelayCommand OpenApplicationCommand { get; private set; }

        public RelayCommand ReloadCommand { get; private set; }

        public RelayCommand OpenAdminCommand { get; private set; }

        public RelayCommand ShowTaskCommand { get; private set; }

        public RelayCommand ReNewCodeForGroup { get; private set; }


        private void SelectAppAndTask()
        {
            m_dataService.SetRestoringLastGroupState(true);
            m_dataService.SetAppSelectionTarget(SelectApplicationTarget.SelectTask);
            Navigate<SelectApplicationView>();
        }

        private void ChangeGroupState(GroupStateContract newState)
        {
            SavingState = true;
            m_dataService.UpdateGroupState(GroupInfo.GroupId, newState, exception =>
            {
                SavingState = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                GroupStateUpdated(newState);
            });
            IsGlobalFocus = true;
        }

        private void RemoveGroup()
        {
            IsRemoveFlyoutOpen = false;

            Removing = true;
            m_dataService.RemoveGroup(GroupInfo.GroupId, exception =>
            {
                Removing = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

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
            m_dataService.SetAppSelectionTarget(SelectApplicationTarget.None);
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
                    if (exception is InvalidServerOperationException)
                        CanConnectToGroup = true;
                    else
                        m_errorService.ShowConnectionError();
                }
            });
        }

        private void OpenAdminView()
        {
            var taskInfo = m_groupInfo.Task;
            if (taskInfo == null)
            {
                Navigate<AdminHostView>();
                return;
            }

            m_dataService.GetApplication(taskInfo.Application, (appInfo, exception) =>
            {
                if (appInfo != null && appInfo.AdminDataTemplate == null)
                {
                    Navigate<ApplicationHostView>();
                }
                else
                {
                    Navigate<AdminHostView>();
                }
            });
        }

        private void ShowTask()
        {
            m_navigationService.OpenPopup<TaskPreviewHostView>();
            Messenger.Default.Send(new SelectedTaskMessage {TaskViewModel = m_groupInfo.Task});
        }

        private void RenewInputCode()
        {
            RenewCodeInProgress = true;
            ShowRenewError = false;
            m_dataService.RenewCodeForGroup(GroupInfo.GroupId, (newGroupCode, exception) =>
            {
                RenewCodeInProgress = false;
                if (exception != null || string.IsNullOrWhiteSpace(newGroupCode))
                {
                    if (exception is InvalidServerOperationException)
                        ShowRenewError = true;
                    else
                        m_errorService.ShowConnectionError();

                    return;
                }

                GroupInfo.GroupCode = newGroupCode;
            });
        }

        public bool ShowRenewError
        {
            get { return m_showRenewError; }
            set
            {
                m_showRenewError = value;
                RaisePropertyChanged();
            }
        }

        public bool RenewCodeInProgress
        {
            get { return m_renewCodeInProgress; }
            set
            {
                m_renewCodeInProgress = value;
                RaisePropertyChanged();
            }
        }
    }
}