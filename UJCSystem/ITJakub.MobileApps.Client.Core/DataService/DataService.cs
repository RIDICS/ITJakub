using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private readonly LoginManager m_loginManager;
        private readonly ApplicationManager m_applicationManager;
        private SynchronizeManager m_synchronizeManager;
        private MobileAppsServiceClient m_serviceClient;

        public DataService()
        {
            m_serviceClient = new MobileAppsServiceClient();
            m_loginManager = new LoginManager();
            m_applicationManager = new ApplicationManager();
            m_synchronizeManager = SynchronizeManager.Instance;
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
        {
            m_applicationManager.GetAllApplicationViewModels(callback);
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            //TODO load messages from server
            callback(new ObservableCollection<MessageViewModel>(), null);
        }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            m_applicationManager.GetAllApplications(callback);
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            var application = m_applicationManager.GetApplication(type);
            callback(application, null);
        }

        public void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            //TODO load group list from server
            var list = new ObservableCollection<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    ApplicationType = ApplicationType.SampleApp,
                    GroupCode = "123546",
                    MemberCount = 5,
                    GroupName = "Group A"
                },
                new GroupInfoViewModel
                {
                    ApplicationType = ApplicationType.Hangman,
                    GroupCode = "123546",
                    MemberCount = 5,
                    GroupName = "Group B"
                },
            };
            foreach (var group in list)
            {
                var application = m_applicationManager.GetApplication(group.ApplicationType);
                group.Icon = application.Icon;
                group.ApplicationName = application.Name;
            }
            callback(list, null);
        }

        public async void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback)
        {
            UserInfo userInfo = null;
            switch (loginProvider)
            {
                case LoginProvider.LiveId:
                    userInfo = await m_loginManager.LoginLiveId();
                    break;
                case LoginProvider.Facebook:
                    userInfo = await m_loginManager.LoginFacebookAsync();
                    break;
                case LoginProvider.Google:
                    userInfo = await m_loginManager.LoginGoogle();
                    break;
            }

            if (userInfo != null)
            {
                callback(userInfo, null);
            }
        }
    }
}