using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Chat.Message;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.Shared.Message;
using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationHostViewModel : ViewModelBase
    {
        private const PollingInterval GroupStatePollingInterval = PollingInterval.Medium;
        private const PollingInterval GroupMembersPollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;
        private readonly IErrorService m_errorService;

        private string m_applicationName;
        private ApplicationBaseViewModel m_applicationViewModel;
        private SupportAppBaseViewModel m_chatViewModel;

        private bool m_isChatDisplayed;
        private bool m_isChatSupported;
        private bool m_isCommandBarOpen;
        private bool m_waitingForStart;
        private bool m_waitingForData;
        private bool m_isTaskAndAppLoaded;
        private bool m_isAppStarted;
        private bool m_isPaused;
        private int m_unreadMessageCount;
        private GroupInfoViewModel m_groupInfo;
        private ObservableCollection<GroupMemberViewModel> m_memberList;
        private TaskViewModel m_currentTask;
        private bool m_isClosed;
        private bool m_isCommunicationStopped;

        public ApplicationHostViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;
            m_errorService = errorService;

            GoBackCommand = new RelayCommand(GoBack);
            ShowChatCommand = new RelayCommand(() => IsChatDisplayed = true);

            m_isCommunicationStopped = false;
            m_isTaskAndAppLoaded = false;
            m_unreadMessageCount = 0;

            LoadData();
            RegisterForMessages();
        }

        private void LoadData()
        {
            m_dataService.GetCurrentGroupId((groupId, groupType) =>
            {
                WaitingForStart = true;
                WaitingForData = true;

                m_groupInfo = new GroupInfoViewModel
                {
                    GroupId = groupId,
                    GroupType = GroupType.Owner
                };

                m_pollingService.RegisterForGetGroupState(GroupStatePollingInterval, groupId, GroupStateUpdate);

                IsOwnerMode = groupType == GroupType.Owner;
                if (IsOwnerMode)
                {
                    // start polling new group members
                    m_pollingService.RegisterForGroupsUpdate(GroupMembersPollingInterval, new[] { m_groupInfo }, GroupsUpdate);
                }
            });
        }

        private void RegisterForMessages()
        {
            Messenger.Default.Register<LogOutMessage>(this, message => StopCommunication());

            Messenger.Default.Register<NotifyNewMessagesMessage>(this, message =>
            {
                if (!IsChatDisplayed)
                    UnreadMessageCount += message.Count;
            });

            Messenger.Default.Register<AppActionFinishedMessage>(this, message =>
            {
                IsCommandBarOpen = false;
            });
        }

        private void GroupsUpdate(Exception exception)
        {
            if (exception != null)
                m_errorService.ShowConnectionWarning();
            else
                DispatcherHelper.CheckBeginInvokeOnUI(() => MemberList = m_groupInfo.Members);
        }

        private void StopCommunication()
        {
            m_isCommunicationStopped = true;
            WaitingForStart = false;
            WaitingForData = false;
            m_pollingService.Unregister(GroupStatePollingInterval, GroupStateUpdate);
            m_pollingService.Unregister(GroupMembersPollingInterval, GroupsUpdate);
            Messenger.Default.Unregister(this);

            if (ChatApplicationViewModel != null)
                ChatApplicationViewModel.StopCommunication();
            
            if (ApplicationViewModel != null)
                ApplicationViewModel.StopCommunication();

            m_pollingService.UnregisterAll(); //stop all polling (in case that some app didn't stopped)
        }

        private void GoBack()
        {
            StopCommunication();
            m_navigationService.GoBack();
        }

        private void GroupStateUpdate(GroupStateContract state, Exception exception)
        {
            if (exception != null)
            {
                m_errorService.ShowConnectionWarning();
                return;
            }

            if (m_isCommunicationStopped)
            {
                return;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                m_groupInfo.State = state;
                GroupStateUpdate();
            });
        }

        private void GroupStateUpdate()
        {
            var state = m_groupInfo.State;
            if (!m_isTaskAndAppLoaded && state >= GroupStateContract.WaitingForStart)
            {
                LoadTask();
                return;
            }

            if (!m_isAppStarted && state >= GroupStateContract.Running)
            {
                WaitingForStart = false;
                StartMainApplication();
                return;
            }

            IsPaused = state == GroupStateContract.Paused;

            if (state == GroupStateContract.Closed)
            {
                IsClosed = true;
                IsChatDisplayed = false;
                IsChatSupported = false;
                UnreadMessageCount = 0;
                ApplicationViewModel.EvaluateAndShowResults();
                m_pollingService.Unregister(GroupStatePollingInterval, GroupStateUpdate);
            }
        }

        private void LoadTask()
        {
            m_dataService.GetTaskForGroup(m_groupInfo.GroupId, (task, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowConnectionWarning();
                    return;
                }

                m_currentTask = task;
                LoadApplications(task.Application);
            });
        }

        private void LoadApplications(ApplicationType type)
        {
            WaitingForData = true;
            m_dataService.GetApplicationByTypes(new List<ApplicationType> { ApplicationType.Chat, type }, (applications, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowError("Tato skupina obsahuje neznámé zadání. Pro otevření této skupiny použijte jinou aplikaci.", "Neznámá aplikace", GoBack);
                    return;
                }

                var application = applications[type];
                ApplicationViewModel = application.ApplicationViewModel;
                ApplicationViewModel.DataLoadedCallback = () => WaitingForData = false;
                ApplicationViewModel.GoBack = GoBack;
                
                ApplicationName = application.Name;
                IsChatSupported = application.IsChatSupported;

                if (IsChatSupported)
                {
                    var chat = applications[ApplicationType.Chat];
                    ChatApplicationViewModel = chat.ApplicationViewModel as SupportAppBaseViewModel;
                    if (ChatApplicationViewModel != null)
                    {
                        ChatApplicationViewModel.InitializeCommunication();
                    }
                }

                m_isTaskAndAppLoaded = true;
                GroupStateUpdate();
            });
        }

        private void StartMainApplication()
        {
            if (m_currentTask.Data == null)
            {
                m_errorService.ShowError("Nebyla přijata žádná data potřebná pro zobrazení aplikace s konkrétním zadáním (úlohou). Aplikace byla ukončena.", "Nepřijata žádná data");
                GoBack();
                return;
            }

            ApplicationViewModel.SetTask(m_currentTask.Data);
            ApplicationViewModel.InitializeCommunication();
            
            m_isAppStarted = true;
            GroupStateUpdate();
        }

        #region Properties

        public string ApplicationName
        {
            get { return m_applicationName; }
            set
            {
                m_applicationName = value;
                RaisePropertyChanged();
            }
        }

        public ApplicationBaseViewModel ApplicationViewModel
        {
            get { return m_applicationViewModel; }
            set 
            { 
                m_applicationViewModel = value;
                RaisePropertyChanged();
            }
        }

        public SupportAppBaseViewModel ChatApplicationViewModel
        {
            get { return m_chatViewModel; }
            set { m_chatViewModel = value;RaisePropertyChanged(); }
        }

        public bool IsChatSupported
        {
            get { return m_isChatSupported; }
            protected set
            {
                m_isChatSupported = value;
                RaisePropertyChanged();
            }
        }

        public bool IsChatDisplayed
        {
            get { return m_isChatDisplayed; }
            set
            {
                m_isChatDisplayed = value;
                RaisePropertyChanged();
                
                ChatApplicationViewModel.AppVisibilityChanged(m_isChatDisplayed);

                if (m_isChatDisplayed)
                {
                    IsCommandBarOpen = false;
                    UnreadMessageCount = 0;
                }
            }
        }

        public bool IsCommandBarOpen
        {
            get { return m_isCommandBarOpen; }
            set
            {
                m_isCommandBarOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool WaitingForStart
        {
            get { return m_waitingForStart; }
            set
            {
                m_waitingForStart = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => Waiting);
            }
        }

        public bool WaitingForData
        {
            get { return m_waitingForData; }
            set
            {
                m_waitingForData = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => Waiting);
            }
        }

        public bool IsPaused
        {
            get { return m_isPaused; }
            set
            {
                m_isPaused = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => Waiting);
            }
        }

        public bool IsClosed
        {
            get { return m_isClosed; }
            set
            {
                m_isClosed = value;
                RaisePropertyChanged();
            }
        }

        public bool Waiting
        {
            get { return m_waitingForStart || m_waitingForData || m_isPaused; }
        }

        public int UnreadMessageCount
        {
            get { return m_unreadMessageCount; }
            set
            {
                m_unreadMessageCount = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => IsChatNotificationVisible);
            }
        }

        public bool IsChatNotificationVisible
        {
            get { return m_unreadMessageCount > 0; }
        }

        public ObservableCollection<GroupMemberViewModel> MemberList
        {
            get { return m_memberList; }
            set
            {
                m_memberList = value;
                RaisePropertyChanged();
            }
        }

        public bool IsOwnerMode { get; set; }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand ShowChatCommand { get; private set; }


        #endregion
    }
}
