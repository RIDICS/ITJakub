using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_applicationName;
        private ApplicationBaseViewModel m_applicationViewModel;
        private ApplicationBaseViewModel m_chatViewModel;

        private bool m_isChatDisplayed;
        private bool m_isChatSupported;
        private bool m_isCommandBarOpen;
        


        public ApplicationHostViewModel(IDataService dataService)
        {
            m_dataService = dataService;
        }

        //TODO tak tohle je humus volat z callbehindu
        public void LoadInitData(ApplicationType type)
        {
<<<<<<< HEAD
            m_dataService.GetApplicationByTypes(new List<ApplicationType> { ApplicationType.Chat, type }, (applications, exception) =>
=======

            //TODO load by multiple types together 
            m_dataService.GetApplication(ApplicationType.Chat, (application, exception) =>
            {
                if(exception!= null)
                    return;

                ChatApplicationViewModel = application.ApplicationViewModel;
            });
            m_dataService.GetApplication(type, (application, exception) =>
>>>>>>> 76f07b70317554fb477fd5225878b9cf1ddc05ba
            {
                if (exception != null)
                    return;


                var application = applications[type];
                ApplicationViewModel = application.ApplicationViewModel;
                ApplicationName = application.Name;
                IsChatSupported = application.IsChatSupported;

                var chat = applications[ApplicationType.Chat];
                ChatApplicationViewModel = chat.ApplicationViewModel;
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
            set { 
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
    }
}
