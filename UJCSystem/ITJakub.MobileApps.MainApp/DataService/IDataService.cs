using System;
using ITJakub.MobileApps.MainApp.Enum;
using ITJakub.MobileApps.MainApp.ViewModel;

namespace ITJakub.MobileApps.MainApp.DataService
{
    public interface IDataService
    {
        void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
    }
}