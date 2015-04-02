using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class NavigationService : INavigationService
    {

        public virtual bool CanGoBack
        {
            get
            {
                var frame = ((Frame)Window.Current.Content);
                return frame.CanGoBack;
            }
        }

        public Type CurrentPageType
        {
            get
            {
                var frame = ((Frame)Window.Current.Content);
                return frame.CurrentSourcePageType;
            }
        }

        public virtual void GoBack()
        {
            var frame = ((Frame)Window.Current.Content);
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        public virtual void GoForward()
        {
            var frame = ((Frame)Window.Current.Content);
            if (frame.CanGoForward)
            {
                frame.GoForward();
            }
        }

        public virtual void GoHome()
        {
            var frame = ((Frame)Window.Current.Content);
            if (frame.CanGoBack)
            {
                var rootPage = frame.BackStack[0];
                frame.Navigate(rootPage.SourcePageType);

                frame.BackStack.Clear();
                frame.ForwardStack.Clear();
            }
        }

        public virtual void Navigate(Type sourcePageType)
        {
            var frame = ((Frame)Window.Current.Content);
            frame.Navigate(sourcePageType);
        }

        public virtual void Navigate(Type sourcePageType, object parameter)
        {
            var frame = ((Frame)Window.Current.Content);
            frame.Navigate(sourcePageType, parameter);
        }

        public void Navigate<T>()
        {
            Navigate(typeof(T));
        }

        public void Navigate<T>(object parameter)
        {
            Navigate(typeof(T), parameter);
        }
    }
}