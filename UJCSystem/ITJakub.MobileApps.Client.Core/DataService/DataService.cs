using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        private readonly MobileAppsServiceManager m_serviceManager;
        //private SynchronizeManager m_synchronizeManager;
        private readonly AuthenticationManager m_authenticationManager;

        public DataService(AuthenticationManager authenticationManager, ApplicationManager applicationManager, MobileAppsServiceManager serviceManager)
        {
            m_authenticationManager = authenticationManager;
            m_applicationManager = applicationManager;
            m_serviceManager = serviceManager;
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

        public void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            var applications = m_applicationManager.GetAllApplicationsByTypes(types);
            callback(applications, null);
        }

        public async void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceManager.GetGroupListAsync(m_authenticationManager.UserInfo.UserId);
                callback(list, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }

            //TODO load group list from server
            //var list = new ObservableCollection<GroupInfoViewModel>
            //{
            //    new GroupInfoViewModel
            //    {
            //        ApplicationType = ApplicationType.SampleApp,
            //        GroupCode = "123546",
            //        MemberCount = 5,
            //        GroupName = "Group A"
            //    },
            //    new GroupInfoViewModel
            //    {
            //        ApplicationType = ApplicationType.Hangman,
            //        GroupCode = "123546",
            //        MemberCount = 5,
            //        GroupName = "Group B"
            //    },
            //};
            //foreach (var group in list)
            //{
            //    var application = m_applicationManager.GetApplication(group.ApplicationType);
            //    group.Icon = application.Icon;
            //    group.ApplicationName = application.Name;
            //}
            
        }

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            m_authenticationManager.GetAllLoginProviderViewModels(callback);
        }

        public async void CreateNewGroup(string groupName, Action<CreateGroupResult, Exception> callback)
        {
            try
            {
                var result = await m_serviceManager.CreateGroupAsync(m_authenticationManager.UserInfo.UserId, groupName);
                callback(result, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void ConnectToGroup(string code, Action<Exception> callback)
        {
            try
            {
                await m_serviceManager.AddUserToGroupAsync(code, m_authenticationManager.UserInfo.UserId);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
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