using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        //private SynchronizeManager m_synchronizeManager;
        private readonly UserManager m_userManager;
        private readonly AuthenticationManager m_authenticationManager;

        public DataService(AuthenticationManager authenticationManager, UserManager userManager, ApplicationManager applicationManager)
        {
            m_authenticationManager = authenticationManager;
            m_userManager = userManager;
            m_applicationManager = applicationManager;
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
        {
            m_applicationManager.GetAllApplicationViewModels(callback);
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

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            m_authenticationManager.GetAllLoginProviderViewModels(callback);
        }

        public async void Login(LoginProviderType loginProviderType, Action<UserInfo, Exception> callback)
        {
            try
            {
                var userInfo = await m_authenticationManager.LoginAsync(loginProviderType);
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

        public async void CreateUser(LoginProviderType loginProviderType, Action<UserInfo, Exception> callback)
        {
            try
            {
                var userInfo = await m_authenticationManager.CreateUserAsync(loginProviderType);
                callback(userInfo, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public UserInfo GetUserInfo()
        {
            return m_authenticationManager.UserInfo;
        }

        public void LogOut()
        {
            m_authenticationManager.LogOut();
        }
    }
}