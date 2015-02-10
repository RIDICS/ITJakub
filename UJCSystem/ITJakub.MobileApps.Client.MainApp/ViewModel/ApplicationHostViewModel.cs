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
using ITJakub.MobileApps.DataContracts;
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
        private string m_applicationName;
        private ApplicationBaseViewModel m_applicationViewModel;
        private SupportAppBaseViewModel m_chatViewModel;

        private bool m_isChatDisplayed;
        private bool m_isChatSupported;
        private bool m_isCommandBarOpen;
        private bool m_waitingForStart;
        private bool m_waitingForData;
        private bool m_taskLoaded;
        private long m_groupId;
        private bool m_paused;
        private int m_unreadMessageCount;
        private GroupInfoViewModel m_groupInfo;
        private ObservableCollection<GroupMemberViewModel> m_memberList;

        public ApplicationHostViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;

            GoBackCommand = new RelayCommand(GoBack);
            ShowChatCommand = new RelayCommand(() => IsChatDisplayed = true);

            m_taskLoaded = false;
            m_unreadMessageCount = 0;

            LoadData();
            RegisterForMessages();
        }

        private void LoadData()
        {
            m_dataService.GetCurrentGroupId(groupId =>
            {
                WaitingForStart = true;
                WaitingForData = true;
                m_groupId = groupId;
                m_pollingService.RegisterForGetGroupState(GroupStatePollingInterval, groupId, GroupStateUpdate);
            });

            m_dataService.GetLoggedUserInfo(false, user =>
            {
                IsTeacherMode = user.UserRole == UserRoleContract.Teacher;

                if (IsTeacherMode)
                {
                    // start polling new group members
                    m_groupInfo = new GroupInfoViewModel
                    {
                        GroupId = m_groupId,
                        GroupType = GroupType.Owner

                    };
                    m_pollingService.RegisterForGroupsUpdate(GroupMembersPollingInterval, new []{m_groupInfo}, GroupsUpdate);
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
            MemberList = m_groupInfo.Members;
        }

        private void StopCommunication()
        {
            WaitingForStart = false;
            WaitingForData = false;
            m_pollingService.Unregister(GroupStatePollingInterval, GroupStateUpdate);
            m_pollingService.Unregister(GroupMembersPollingInterval, GroupsUpdate);
            Messenger.Default.Unregister(this);

            if (ChatApplicationViewModel != null)
                ChatApplicationViewModel.StopCommunication();
            
            if (ApplicationViewModel != null)
                ApplicationViewModel.StopCommunication();
        }

        private void GoBack()
        {
            StopCommunication();
            m_navigationService.GoBack();
        }

        private void GroupStateUpdate(GroupStateContract state, Exception exception)
        {
            if (exception != null)
                return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (state >= GroupStateContract.WaitingForStart && !m_taskLoaded)
                {
                    LoadTask();
                }

                if (state >= GroupStateContract.Running)
                {
                    m_pollingService.Unregister(GroupStatePollingInterval, GroupStateUpdate);
                    WaitingForStart = false;
                }
            });
        }

        private void LoadTask()
        {
            m_dataService.GetTaskForGroup(m_groupId, (task, exception) =>
            {
                if (exception != null)
                    return;

                LoadApplications(task.Application, task.Data);
                m_taskLoaded = true;
            });
        }

        private void LoadApplications(ApplicationType type, string taskData)
        {
            WaitingForData = true;
            m_dataService.GetApplicationByTypes(new List<ApplicationType> { ApplicationType.Chat, type }, (applications, exception) =>
            {
                if (exception != null)
                    return;

                var application = applications[type];
                ApplicationViewModel = application.ApplicationViewModel;
                ApplicationViewModel.DataLoadedCallback = () => WaitingForData = false;
                ApplicationViewModel.SetTask(taskData);
                ApplicationViewModel.InitializeCommunication();
                ApplicationName = application.Name;
                IsChatSupported = application.IsChatSupported;

                if (!IsChatSupported)
                    return;
                
                var chat = applications[ApplicationType.Chat];
                ChatApplicationViewModel = chat.ApplicationViewModel as SupportAppBaseViewModel;
                if (ChatApplicationViewModel != null)
                {
                    ChatApplicationViewModel.InitializeCommunication();
                }
            });
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

        public bool Paused
        {
            get { return m_paused; }
            set
            {
                m_paused = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => Waiting);
            }
        }

        public bool Waiting
        {
            get { return m_waitingForStart || m_waitingForData; }
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

        public bool IsTeacherMode { get; set; }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand ShowChatCommand { get; private set; }

        #endregion
    }
}
