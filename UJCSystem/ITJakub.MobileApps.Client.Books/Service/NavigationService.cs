using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class NavigationService
    {
        private int m_originalBackStackDepth;
        private object m_originalFrameContent;

        public NavigationService()
        {
            SetRootPage();
        }

        public void SetRootPage()
        {
            var frame = ((Frame)Window.Current.Content);
            m_originalFrameContent = frame.Content;
            m_originalBackStackDepth = frame.BackStackDepth;
        }

        public bool CanGoBack
        {
            get
            {
                var frame = ((Frame)Window.Current.Content);
                return m_originalBackStackDepth < frame.BackStackDepth;
            }
        }

        public void GoBack()
        {
            if (!CanGoBack)
                return;

            var frame = ((Frame)Window.Current.Content);

            if (m_originalBackStackDepth + 1 == frame.BackStackDepth)
            {
                GoFromBookSelection();
                return;
            }

            frame.GoBack();
        }

        public void GoFromBookSelection()
        {
            var frame = ((Frame) Window.Current.Content);
            while (m_originalBackStackDepth < frame.BackStackDepth)
            {
                frame.GoBack();
                frame = ((Frame) Window.Current.Content);
            }
            frame.Content = m_originalFrameContent;
        }

        public void Navigate(Type sourcePageType)
        {
            var frame = ((Frame)Window.Current.Content);
            frame.Navigate(sourcePageType);
        }

        public void Navigate(Type sourcePageType, object parameter)
        {
            var frame = ((Frame)Window.Current.Content);
            frame.Navigate(sourcePageType, parameter);
        }
    }
}