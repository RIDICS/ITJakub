using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class NavigationService : INavigationService
    {
        private readonly Popup m_popup;

        public NavigationService()
        {
            m_popup = new Popup();
            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack, ()=>CanGoBack);
            GoHomeCommand = new RelayCommand(GoHome);
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
                frame.GoBack();
            }
        }

        public RelayCommand GoBackCommand { get; private set; }

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

        public RelayCommand GoHomeCommand { get; private set; }

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

        public void OpenPopup<T>()
        {
            var newPage = (Page) Activator.CreateInstance(typeof (T));
            newPage.Width = Window.Current.Bounds.Width;
            newPage.Height = Window.Current.Bounds.Height;
            m_popup.Child = newPage;
            m_popup.IsOpen = true;

            Window.Current.SizeChanged += UpdatePopupSize;
        }

        private void UpdatePopupSize(object sender, WindowSizeChangedEventArgs e)
        {
            var page = m_popup.Child as Page;
            if (page == null)
                return;

            page.Width = Window.Current.Bounds.Width;
            page.Height = Window.Current.Bounds.Height;
        }

        public void ClosePopup()
        {
            m_popup.IsOpen = false;
            m_popup.Child = null;
            Window.Current.SizeChanged -= UpdatePopupSize;
        }
    }
}