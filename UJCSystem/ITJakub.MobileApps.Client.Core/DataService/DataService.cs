using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly GroupManager m_groupManager;

        public DataService(AuthenticationManager authenticationManager, ApplicationManager applicationManager,
            GroupManager groupManager)
        {
            m_authenticationManager = authenticationManager;
            m_applicationManager = applicationManager;
            m_groupManager = groupManager;
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

        public void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            m_groupManager.GetGroupForCurrentUser(callback);
        }

        public void OpenGroupAndGetDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            m_groupManager.OpenGroupAndGetDetails(groupId, callback);
        }

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            m_authenticationManager.GetAllLoginProviderViewModels(callback);
        }

        public void CreateNewGroup(string groupName, Action<CreateGroupResponse, Exception> callback)
        {
            m_groupManager.CreateNewGroup(groupName, callback);
        }

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            m_groupManager.ConnectToGroup(code, callback);
        }

        public void LoadGroupMemberAvatars(IList<GroupMemberViewModel> groupMember)
        {
            m_groupManager.LoadGroupMemberAvatars(groupMember);
        }

        public void UpdateGroupMembers(GroupInfoViewModel group)
        {
            m_groupManager.UpdateGroupNewMembers(group);
        }

        public void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            m_authenticationManager.LoginByProvider(loginProviderType, callback);
        }

        public void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            m_authenticationManager.CreateUserByLoginProvider(loginProviderType, callback);
        }

        public void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback)
        {
            m_authenticationManager.GetLoggedUserInfo(callback);
        }

        public void LogOut()
        {
            m_authenticationManager.LogOut();
        }
    }
}