using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        //private SynchronizeManager m_synchronizeManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly MobileAppsServiceManager m_serviceManager;
        private GroupManager m_groupManager;

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
            ApplicationBase application = m_applicationManager.GetApplication(type);
            callback(application, null);
        }

        public void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            Dictionary<ApplicationType, ApplicationBase> applications = m_applicationManager.GetAllApplicationsByTypes(types);
            callback(applications, null);
        }

        public async void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            m_groupManager.GetGroupForCurrentUser(callback);
        }

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            m_authenticationManager.GetAllLoginProviderViewModels(callback);
        }

        public async void CreateNewGroup(string groupName, Action<CreateGroupResult, Exception> callback)
        {
            m_groupManager.CreateNewGroup(groupName, callback);
        }

        public async void ConnectToGroup(string code, Action<Exception> callback)
        {
            m_groupManager.ConnectToGroup(code, callback);
          
        }

        public void Login(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            m_authenticationManager.LoginByProvider(loginProviderType, callback);
        }

        public void CreateUser(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            m_authenticationManager.CreateUserByLoginProvider(loginProviderType, callback);
        }

        public void GetLogedUserInfo(Action<LogedUserViewModel, Exception> callback)
        {
            m_authenticationManager.GetLogedUserInfo(callback);
        }

        public void LogOut()
        {
            m_authenticationManager.LogOut();
        }
    }
}