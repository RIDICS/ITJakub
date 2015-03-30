using System;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public interface IErrorService
    {
        void ShowConnectionError();
        void ShowConnectionWarning();
        void ShowError(string content, string title = null, Action closeAction = null);
    }
}