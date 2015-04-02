using System;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public interface INavigationService
    {
        bool CanGoBack
        {
            get;
        }

        Type CurrentPageType
        {
            get;
        }

        void GoBack();
        void GoForward();
        void GoHome();
        void Navigate(Type sourcePageType);
        void Navigate(Type sourcePageType, object parameter);
        void Navigate<T>();
        void Navigate<T>(object parameter);
    }
}