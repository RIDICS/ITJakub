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
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public interface IDataService
    {
        void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback);
        void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback);
        void GetLoggedUserInfo(bool getUserAvatar, Action<LoggedUserViewModel> callback);
        void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback);
        void LogOut();
        
        void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback);
        void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        
        void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback);
        void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback);
        void CreateNewGroup(string groupName, Action<CreatedGroupViewModel, Exception> callback);
        void ConnectToGroup(string code, Action<Exception> callback);
        void UpdateGroupState(long groupId, GroupStateContract newState, Action<Exception> callback);
        void RemoveGroup(long groupId, Action<Exception> callback);

        void GetTask(long taskId, Action<TaskViewModel, Exception> callback);
        void GetTaskForGroup(long groupId, Action<TaskViewModel, Exception> callback);
        void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback);
        void GetMyTasks(Action<ObservableCollection<TaskViewModel>, Exception> callback);
        void AssignTaskToCurrentGroup(long taskId, Action<Exception> callback);
        
        void SetCurrentGroup(long groupId, GroupType groupType);
        void GetCurrentGroupId(Action<long, GroupType> callback);
        void SetCurrentApplication(ApplicationType selectedApp);
        void GetCurrentApplication(Action<ApplicationType> callback);
        void SetRestoringLastGroupState(bool restore);
        void GetAppSelectionTarget(Action<SelectApplicationTarget> callback);
        void SetAppSelectionTarget(SelectApplicationTarget target);
    }
}