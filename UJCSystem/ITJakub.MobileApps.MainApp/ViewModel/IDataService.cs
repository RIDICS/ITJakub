using System;
using ITJakub.MobileApps.MainApp.Enum;

namespace ITJakub.MobileApps.MainApp.ViewModel
{
    public interface IDataService
    {
        void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
    }
}