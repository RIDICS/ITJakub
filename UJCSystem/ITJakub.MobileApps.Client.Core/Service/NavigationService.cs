using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<Type> m_pageTypeStack;
        private readonly Stack<object> m_viewModelStack;

        public NavigationService()
        {
            m_pageTypeStack = new Stack<Type>();
            m_viewModelStack = new Stack<object>();
        }

        public virtual bool CanGoBack
        {
            get
            {
                return m_pageTypeStack.Count > 0;
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

            if (CanGoBack)
            {
                frame.Navigate(m_pageTypeStack.Pop());
                m_viewModelStack.Pop();

                frame.BackStack.Clear();
            }
        }

        public void GoBackUsingCache()
        {
            var frame = ((Frame)Window.Current.Content);

            if (CanGoBack)
            {
                frame.Navigate(m_pageTypeStack.Pop());

                var newPage = (Page) frame.Content;
                var dataContext = m_viewModelStack.Pop();
                if (newPage != null) 
                    newPage.DataContext = dataContext;

                frame.BackStack.Clear();
            }
        }

        public virtual void GoForward()
        {
            throw new NotSupportedException();
        }

        public virtual void GoHome()
        {
            var frame = ((Frame)Window.Current.Content);

            if (m_pageTypeStack.Count == 0)
                return;

            while (m_pageTypeStack.Count > 0)
                m_pageTypeStack.Pop();
            
            frame.Navigate(m_pageTypeStack.Pop());
            
            m_pageTypeStack.Clear();
            m_viewModelStack.Clear();
        }

        public virtual void Navigate(Type sourcePageType)
        {
            var frame = ((Frame)Window.Current.Content);
            var page = (Page) frame.Content;

            m_pageTypeStack.Push(frame.CurrentSourcePageType);
            m_viewModelStack.Push(page != null ? page.DataContext : null);
            frame.Navigate(sourcePageType);
        }

        public virtual void Navigate(Type sourcePageType, object parameter)
        {
            var frame = ((Frame)Window.Current.Content);
            var page = (Page) frame.Content;

            m_pageTypeStack.Push(frame.CurrentSourcePageType);
            m_viewModelStack.Push(page != null ? page.DataContext : null);
            frame.Navigate(sourcePageType, parameter);
        }
    }
}