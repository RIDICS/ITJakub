using System;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.Service;
using ITJakub.MobileApps.Client.Books.View;
using ITJakub.MobileApps.Client.Books.ViewModel;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Books
{
    public class Book
    {
        private Action<BookPageViewModel> m_callback;
        private readonly NavigationService m_navigationService;

        public Book()
        {
            m_navigationService = Container.Current.Resolve<NavigationService>();
        }

        public void SelectBook(Action<BookPageViewModel> callback)
        {
            m_callback = callback;
            Messenger.Default.Register<SelectedPageMessage>(this, BookSelected);
            m_navigationService.SetRootPage();
            m_navigationService.Navigate(typeof(SelectBookView));
        }

        private void BookSelected(SelectedPageMessage message)
        {
            Messenger.Default.Unregister(this);
            m_callback(message.BookPage);
        }
    }
}
