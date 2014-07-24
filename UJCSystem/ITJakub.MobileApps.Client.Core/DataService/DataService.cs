using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private LoginManager m_loginManager;
        private ApplicationManager m_applicationManager;

        public DataService()
        {
            m_loginManager = new LoginManager();
            m_applicationManager = new ApplicationManager();
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
        {
            m_applicationManager.GetAllApplicationViewModels(callback);
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            callback(new ObservableCollection<MessageViewModel>(), null);
        }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            m_applicationManager.GetAllApplications(callback);
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            m_applicationManager.GetApplication(type, callback);
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