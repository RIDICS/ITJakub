using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Manager.News;
using ITJakub.MobileApps.Client.Core.Manager.Tasks;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel.News;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        private readonly ApplicationStateManager m_applicationStateManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly ConfigurationManager m_configurationManager;
        private readonly GroupManager m_groupManager;
        private readonly IMainPollingService m_mainPollingService;
        private readonly NewsManager m_newsManager;
        private readonly TaskManager m_taskManager;

        public DataService(IUnityContainer container)
        {
            m_authenticationManager = container.Resolve<AuthenticationManager>();
            m_applicationManager = container.Resolve<ApplicationManager>();
            m_groupManager = container.Resolve<GroupManager>();
            m_taskManager = container.Resolve<TaskManager>();
            m_mainPollingService = container.Resolve<IMainPollingService>();
            m_applicationStateManager = container.Resolve<ApplicationStateManager>();
            m_newsManager = container.Resolve<NewsManager>();
            m_configurationManager = container.Resolve<ConfigurationManager>();
        }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            m_applicationManager.GetAllApplications(callback);
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            m_applicationManager.GetApplication(type, callback);
        }

        public void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            m_applicationManager.GetAllApplicationsByTypes(types, callback);
        }

        public void GetOwnedGroupsForCurrentUser(Action<List<GroupInfoViewModel>, Exception> callback)
        {
            m_groupManager.GetOwnedGroupsForCurrentUser(callback);
        }

        public Task<ObservableCollection<GroupInfoViewModel>> GetOwnedGroupsForCurrentUserAsync()
        {
            return m_groupManager.GetOwnedGroupsForCurrentUserAsync();
        }

        public void GetGroupsForCurrentUser(Action<List<GroupInfoViewModel>, Exception> callback)
        {
            m_groupManager.GetGroupsForCurrentUser(callback);
        }

        public Task<ObservableCollection<GroupInfoViewModel>> GetGroupForCurrentUserAsync()
        {
            return m_groupManager.GetGroupForCurrentUserAsync();
        }

        public async Task<List<SyndicationItemViewModel>> GetAllNews()
        {            
            return await m_newsManager.GetAllNews();
        }

        public void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            m_groupManager.GetGroupDetails(groupId, callback);
        }

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            m_authenticationManager.GetAllLoginProviderViewModels(callback);
        }

        public void CreateNewGroup(string groupName, Action<CreatedGroupViewModel, Exception> callback)
        {
            m_groupManager.CreateNewGroup(groupName, callback);
        }

        public void DuplicateGroup(long sourceGroupId, string groupName, Action<CreatedGroupViewModel, Exception> callback)
        {
            m_groupManager.DuplicateGroup(sourceGroupId, groupName, callback);
        }

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            m_groupManager.ConnectToGroup(code, callback);
        }

        public void GetTask(long taskId, Action<TaskViewModel, Exception> callback)
        {
            m_taskManager.GetTask(taskId, callback);
        }

        public void GetTaskForGroup(long groupId, Action<TaskViewModel, Exception> callback)
        {
            m_taskManager.GetTaskForGroup(groupId, callback);
        }

        public void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            m_taskManager.GetTasksByApplication(application, callback);
        }

        public void GetMyTasks(Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            m_taskManager.GetMyTasks(callback);
        }

        public void AssignTaskToCurrentGroup(long taskId, Action<Exception> callback)
        {
            m_taskManager.AssignTaskToGroup(m_groupManager.CurrentGroupId, taskId, callback);
        }

        public void SetCurrentGroup(long groupId, GroupType groupType)
        {
            m_groupManager.CurrentGroupId = groupId;
            m_groupManager.CurrentGroupType = groupType;
        }

        public void UpdateGroupState(long groupId, GroupStateContract newState, Action<Exception> callback)
        {
            m_groupManager.UpdateGroupState(groupId, newState, callback);
        }

        public void RemoveGroup(long groupId, Action<Exception> callback)
        {
            m_groupManager.RemoveGroup(groupId, callback);
        }

        public void GetCurrentGroupId(Action<long, GroupType> callback)
        {
            callback(m_groupManager.CurrentGroupId, m_groupManager.CurrentGroupType);
        }

        public void SetCurrentApplication(ApplicationType selectedApp)
        {
            m_applicationManager.CurrentApplication = selectedApp;
        }

        public void GetCurrentApplication(Action<ApplicationType> callback)
        {
            callback(m_applicationManager.CurrentApplication);
        }

        public void SetRestoringLastGroupState(bool restore)
        {
            m_groupManager.RestoreLastState = restore;
        }

        public void GetAppSelectionTarget(Action<SelectApplicationTarget> callback)
        {
            callback(m_applicationStateManager.SelectApplicationTarget);
        }

        public void SetAppSelectionTarget(SelectApplicationTarget target)
        {
            m_applicationStateManager.SelectApplicationTarget = target;
        }

        public void UpdateEndpointAddress(string address)
        {
            m_configurationManager.EndpointAddress = address;
        }

        public void RenewCodeForGroup(long groupId, Action<string, Exception> callback)
        {
            m_groupManager.RenewCodeForGroup(groupId, callback);
        }

        public void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            m_authenticationManager.LoginByProvider(loginProviderType, callback);
        }

        public void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            m_authenticationManager.CreateUserByLoginProvider(loginProviderType, callback);
        }

        public void PromoteUserToTeacherRole(long userId, string promotionCode, Action<bool, Exception> callback)
        {
            m_authenticationManager.PromoteUserToTeacherRole(userId, promotionCode, callback);
        }

        public void GetLoggedUserInfo(bool getUserAvatar, Action<LoggedUserViewModel> callback)
        {
            m_authenticationManager.GetLoggedUserInfo(getUserAvatar, callback);
        }

        public void LogOut()
        {
            m_authenticationManager.LogOut();
            m_mainPollingService.UnregisterAll();
        }
    }
}