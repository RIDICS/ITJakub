using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private string m_applicationName;
        private ApplicationBaseViewModel m_applicationViewModel;
        private ApplicationBaseViewModel m_chatViewModel;

        private bool m_isChatDisplayed;
        private bool m_isChatSupported;
        private bool m_isCommandBarOpen;

        public ApplicationHostViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            GoBackCommand = new RelayCommand(GoBack);
            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                var applicationType = message.Group.ApplicationType;
                //TODO for debug
                if (applicationType == ApplicationType.Unknown)
                    applicationType = ApplicationType.SampleApp;
                LoadApplications(applicationType);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });
        }

        private void GoBack()
        {
            ChatApplicationViewModel.StopTimers();
            ApplicationViewModel.StopTimers();

            if (m_navigationService.CanGoBack)
                m_navigationService.GoBack();
        }

        private void LoadApplications(ApplicationType type)
        {
            m_dataService.GetApplicationByTypes(new List<ApplicationType> { ApplicationType.Chat, type }, (applications, exception) =>
            {
                if (exception != null)
                    return;

                var application = applications[type];
                ApplicationViewModel = application.ApplicationViewModel;
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

        public RelayCommand GoBackCommand { get; private set; }
    }
}
