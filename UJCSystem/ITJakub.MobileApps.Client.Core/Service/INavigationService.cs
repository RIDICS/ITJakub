using System;
using GalaSoft.MvvmLight.Command;

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

        RelayCommand GoBackCommand { get; }

        void GoForward();

        void GoHome();

        RelayCommand GoHomeCommand { get; }

        void Navigate(Type sourcePageType);

        void Navigate(Type sourcePageType, object parameter);

        void Navigate<T>();

        void Navigate<T>(object parameter);

        void OpenPopup<T>();

        void ClosePopup();
    }
}