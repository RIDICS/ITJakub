using System;
using ITJakub.MobileApps.Client.Core.Manager;

namespace ITJakub.MobileApps.Client.MainApp.DataService
{
    public interface IDataService
    {
        void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback);
    }
}