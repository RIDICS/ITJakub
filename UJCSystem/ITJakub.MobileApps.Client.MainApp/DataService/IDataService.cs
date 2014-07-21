using System;
using ITJakub.MobileApps.Client.MainApp.Enum;
using ITJakub.MobileApps.Client.MainApp.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.DataService
{
    public interface IDataService
    {
        void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
    }
}