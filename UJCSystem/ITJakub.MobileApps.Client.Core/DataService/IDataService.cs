using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;


namespace ITJakub.MobileApps.Client.Core.DataService
{
    public interface IDataService
    {
        void Login(LoginProviderType loginProviderType, Action<UserInfo, Exception> callback);
        void CreateUser(LoginProviderType loginProviderType, Action<UserInfo, Exception> callback);
        void GetUserInfo(Action<UserInfo, Exception> callback);
        void LogOut();
        void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback);
        
        void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback);

        void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback);
        void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback);
        void CreateNewGroup(string groupName, Action<CreateGroupResult, Exception> callback);
        void ConnectToGroup(string code, Action<Exception> callback);
    }
}