using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<object> m_frameContentCache;
        private object m_currentParameter;

        public NavigationService()
        {
            m_frameContentCache = new Stack<object>();
            m_currentParameter = null;
        }

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
                if (frame.Content != null)
                {
                    var currentPageEntry = new PageStackEntry(frame.Content.GetType(), m_currentParameter, null);
                    frame.ForwardStack.Add(currentPageEntry);
                }
                var backStackPeek = frame.BackStack[frame.BackStackDepth - 1];
                frame.BackStack.Remove(backStackPeek);
                frame.Content = m_frameContentCache.Pop();
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

            if (frame.BackStackDepth == 0)
                return;

            Type rootPageType = frame.BackStack[0].SourcePageType;
            frame.Navigate(rootPageType);
            frame.BackStack.Clear();
            m_frameContentCache.Clear();
        }

        public virtual void Navigate(Type sourcePageType)
        {
            m_currentParameter = null;
            var frame = ((Frame)Window.Current.Content);

            m_frameContentCache.Push(frame.Content);
            frame.Navigate(sourcePageType);
        }

        public virtual void Navigate(Type sourcePageType, object parameter)
        {
            m_currentParameter = parameter;
            var frame = ((Frame)Window.Current.Content);

            m_frameContentCache.Push(frame.Content);
            frame.Navigate(sourcePageType, parameter);
        }
    }
}