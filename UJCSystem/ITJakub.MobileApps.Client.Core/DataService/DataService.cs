using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        private SynchronizeManager m_synchronizeManager;
        private readonly UserManager m_userManager;

        public DataService()
        {
            var manager = new MobileAppsServiceManager();
            m_applicationManager = new ApplicationManager();
            m_synchronizeManager = SynchronizeManager.Instance;
            m_userManager = new UserManager(manager);
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
        {
            m_applicationManager.GetAllApplicationViewModels(callback);
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            //TODO load messages from server
            //var messageList = m_synchronizeManager.GetSynchronizedObjects(ApplicationType.Chat, new DateTime(), "ChatMessage");
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

        public async void Login(LoginProvider loginProvider, Action<UserInfo, Exception> callback)
        {
            try
            {
                var userInfo = await m_userManager.LoginAsync(loginProvider);
                callback(userInfo, null);
            }
            catch (UserNotRegisteredException exception)
            {
                callback(null, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void CreateUser(LoginProvider loginProvider, Action<UserInfo, Exception> callback)
        {
            try
            {
                var userInfo = await m_userManager.CreateUserAsync(loginProvider);
                callback(userInfo, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public UserInfo GetUserInfo()
        {
            return m_userManager.UserInfo;
        }

        public void LogOut()
        {
            m_userManager.LogOut();
        }
    }
}