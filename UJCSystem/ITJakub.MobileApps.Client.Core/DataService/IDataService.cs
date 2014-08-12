using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;


namespace ITJakub.MobileApps.Client.Core.DataService
{
    public interface IDataService
    {
        void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback);
        void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback);
        void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback);
        void LogOut();
        void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback);
        
        void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback);

        void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback);
        void GetGroupMembers(long groupId, Action<ObservableCollection<GroupMemberViewModel>, Exception> callback);
        void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback);
        void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback);
        void CreateNewGroup(string groupName, Action<CreateGroupResult, Exception> callback);
        void ConnectToGroup(string code, Action<Exception> callback);
    }
}