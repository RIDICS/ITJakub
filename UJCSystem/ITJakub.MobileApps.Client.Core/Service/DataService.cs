using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Manager.Tasks;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class DataService : IDataService
    {
        private readonly ApplicationManager m_applicationManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly GroupManager m_groupManager;
        private readonly TaskManager m_taskManager;
        private readonly IMainPollingService m_mainPollingService;
        private readonly ApplicationStateManager m_applicationStateManager;

        public DataService(IUnityContainer container)
        {
            m_authenticationManager = container.Resolve<AuthenticationManager>();
            m_applicationManager = container.Resolve<ApplicationManager>();
            m_groupManager = container.Resolve<GroupManager>();
            m_taskManager = container.Resolve<TaskManager>();
            m_mainPollingService = container.Resolve<IMainPollingService>();
            m_applicationStateManager = container.Resolve<ApplicationStateManager>();
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

        public void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            m_groupManager.GetGroupForCurrentUser(callback);
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

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            m_groupManager.ConnectToGroup(code, callback);
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

        public void SetCurrentGroup(long groupId)
        {
            m_groupManager.OpenGroup(groupId);
        }

        public void UpdateGroupState(long groupId, GroupState newState, Action<Exception> callback)
        {
            m_groupManager.UpdateGroupState(groupId, newState, callback);
        }

        public void RemoveGroup(long groupId, Action<Exception> callback)
        {
            m_groupManager.RemoveGroup(groupId, callback);
        }

        public void GetCurrentGroupId(Action<long> callback)
        {
            callback(m_groupManager.CurrentGroupId);
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

        public void GetAppSelectionTarget(Action<ApplicationSelectionTarget> callback)
        {
            callback(m_applicationStateManager.ApplicationSelectionTarget);
        }

        public void SetAppSelectionTarget(ApplicationSelectionTarget target)
        {
            m_applicationStateManager.ApplicationSelectionTarget = target;
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
            m_mainPollingService.UnregisterAll();
        }
    }
}