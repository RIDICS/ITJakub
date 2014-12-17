using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationHostViewModel : ViewModelBase
    {
        private const PollingInterval GroupStatePollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;
        private string m_applicationName;
        private ApplicationBaseViewModel m_applicationViewModel;
        private ApplicationBaseViewModel m_chatViewModel;

        private bool m_isChatDisplayed;
        private bool m_isChatSupported;
        private bool m_isCommandBarOpen;
        private bool m_waitingForStart;
        private bool m_waitingForData;
        private bool m_taskLoaded;
        private long m_groupId;
        private bool m_paused;

        public ApplicationHostViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;
            GoBackCommand = new RelayCommand(GoBack);
            m_taskLoaded = false;

            m_dataService.GetCurrentGroupId(groupId =>
            {
                WaitingForStart = true;
                WaitingForData = true;
                m_groupId = groupId;
                m_pollingService.RegisterForGetGroupState(GroupStatePollingInterval, groupId, GroupStateUpdate);
            });

            Messenger.Default.Register<LogOutMessage>(this, message => StopCommunication());
        }

        private void StopCommunication()
        {
            WaitingForStart = false;
            WaitingForData = false;
            m_pollingService.Unregister(GroupStatePollingInterval, GroupStateUpdate);
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

        private void GroupStateUpdate(GroupState state, Exception exception)
        {
            if (exception != null)
                return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (state >= GroupState.WaitingForStart && !m_taskLoaded)
                {
                    LoadTask();
                }

                if (state >= GroupState.Running)
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

                var chat = applications[ApplicationType.Chat];
                ChatApplicationViewModel = chat.ApplicationViewModel;
                ChatApplicationViewModel.InitializeCommunication();
            });
        }

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

        public ApplicationBaseViewModel ChatApplicationViewModel
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
                if (m_isChatDisplayed)
                    IsCommandBarOpen = false;
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

        public RelayCommand GoBackCommand { get; private set; }
    }
}
