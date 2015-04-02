using System;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public interface IErrorService
    {
        void ShowConnectionError(Action closeAction = null);
        void ShowConnectionWarning();
        void ShowError(string content, string title = null, Action closeAction = null);
    }
}