using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationHostViewModel : ViewModelBase
    {
        private const PollingInterval TaskPollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;
        private string m_applicationName;
        private ApplicationBaseViewModel m_applicationViewModel;
        private ApplicationBaseViewModel m_chatViewModel;

        private bool m_isChatDisplayed;
        private bool m_isChatSupported;
        private bool m_isCommandBarOpen;
        private bool m_waitingForTask;
        private bool m_waitingForData;

        public ApplicationHostViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;
            GoBackCommand = new RelayCommand(GoBack);

            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                WaitingForTask = true;
                m_pollingService.RegisterForGetTaskByGroup(TaskPollingInterval, message.Group.GroupId, LoadTask);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });

            Messenger.Default.Register<LogOutMessage>(this, message => StopCommunication());
        }

        private void StopCommunication()
        {
            WaitingForTask = false;
            WaitingForData = false;
            m_pollingService.Unregister(TaskPollingInterval, LoadTask);
            Messenger.Default.Unregister(this);

            if (ChatApplicationViewModel != null)
                ChatApplicationViewModel.StopCommunication();
            
            if (ApplicationViewModel != null)
                ApplicationViewModel.StopCommunication();
        }

        private void GoBack()
        {
            StopCommunication();

            if (m_navigationService.CanGoBack)
                m_navigationService.GoBack();
        }

        private void LoadTask(TaskViewModel task, Exception exception)
        {
            if (exception != null)
                return;

            if (task == null || task.Data == null)
                return;

            m_pollingService.Unregister(TaskPollingInterval, LoadTask);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (!WaitingForTask)
                    return;

                WaitingForTask = false;
                LoadApplications(task.Application, task.Data);
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

        public bool WaitingForTask
        {
            get { return m_waitingForTask; }
            set
            {
                m_waitingForTask = value;
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

        public bool Waiting
        {
            get { return m_waitingForTask || m_waitingForData; }
        }

        public RelayCommand GoBackCommand { get; private set; }
    }
}
