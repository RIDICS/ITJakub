using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public interface IDataService
    {
        void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback);
        void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback);
        void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback);
        void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback);
        void LogOut();
        
        void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback);
        void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        
        void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback);
        void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback);
        void CreateNewGroup(string groupName, Action<CreatedGroupViewModel, Exception> callback);
        void ConnectToGroup(string code, Action<Exception> callback);

        void GetTaskForGroup(long groupId, Action<TaskViewModel, Exception> callback);
        void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback);
        void GetMyTasks(Action<ObservableCollection<TaskViewModel>, Exception> callback);
        void AssignTaskToCurrentGroup(long taskId, Action<Exception> callback);
        void SetCurrentGroup(long groupId);
        void UpdateGroupState(long groupId, GroupState newState, Action<Exception> callback);
        void RemoveGroup(long groupId, Action<Exception> callback);
        //New
        void GetCurrentGroupId(Action<long> callback);
        void SetCurrentApplication(ApplicationType selectedApp);
        void GetCurrentApplication(Action<ApplicationType> callback);
        void SetRestoringLastGroupState(bool restore);
        void GetAppSelectionTarget(Action<ApplicationSelectionTarget> callback);
        void SetAppSelectionTarget(ApplicationSelectionTarget target);
    }
}