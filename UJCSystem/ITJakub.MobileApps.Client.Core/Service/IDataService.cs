using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback);
        void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback);
        void LogOut();
        
        void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback);
        void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        
        void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback);
        void OpenGroupAndGetDetails(long groupId, Action<GroupInfoViewModel, Exception> callback);
        void CreateNewGroup(string groupName, Action<CreateGroupResponse, Exception> callback);
        void ConnectToGroup(string code, Action<Exception> callback);

        void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback);
        void AssignTaskToGroup(long groupId, long taskId, Action<Exception> callback);
        void OpenGroup(long groupId);
        void UpdateGroupState(long groupId, GroupState newState, Action<Exception> callback);
        void RemoveGroup(long groupId, Action<Exception> callback);
    }
}