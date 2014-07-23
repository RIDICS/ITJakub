using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp;
using ITJakub.MobileApps.Client.Shared;


namespace ITJakub.MobileApps.Client.Core.DataService
{
    public interface IDataService
    {
        void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
        void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, object> callback);
        void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback);
    }
}