using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Message;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class NavigationService : INavigationService
    {
        private readonly Popup m_parent;
        private readonly Stack<Type> m_backStack;
        private Type m_currentPageType;

        public NavigationService()
        {
            m_parent = new Popup();
            m_backStack = new Stack<Type>();
        }

        public Popup ParentPopup
        {
            get { return m_parent; }
        }

        private void DisplayPage(Type pageType)
        {
            m_currentPageType = pageType;
            var newPage = (Page) Activator.CreateInstance(pageType);
            newPage.Width = Window.Current.Bounds.Width;
            newPage.Height = Window.Current.Bounds.Height;
            m_parent.Child = newPage;
        }

        public bool CanGoBack()
        {
            return m_backStack.Count > 0;
        }

        public void GoBack()
        {
            if (CanGoBack())
            {
                DisplayPage(m_backStack.Pop());
            }
            else
            {
                m_parent.Child = null;
                m_currentPageType = null;
                Messenger.Default.Send(new CloseBookSelectAppMessage());
            }
        }

        public void Navigate<T>()
        {
            if (m_currentPageType != null)
                m_backStack.Push(m_currentPageType);

            var type = typeof (T);
            DisplayPage(type);
        }

        public void ResetBackStack()
        {
            m_backStack.Clear();
            m_currentPageType = null;
        }
    }
}