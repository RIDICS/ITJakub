using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared;


namespace ITJakub.MobileApps.Client.Core.DataService
{
    public interface IDataService
    {
        void Login(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
        void CreateUser(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
        UserInfo GetUserInfo();
        void LogOut();
        void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback);
        void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback);
        void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback);
        void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback);
        void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback);
    }
}