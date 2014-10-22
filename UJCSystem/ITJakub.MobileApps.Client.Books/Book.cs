using System.Threading.Tasks;
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
        private readonly TaskCompletionSource<BookPageViewModel> m_taskCompletition;
        private readonly NavigationService m_navigationService;

        private Book()
        {
            Messenger.Default.Register<SelectedPageMessage>(this, BookSelected);

            m_taskCompletition = new TaskCompletionSource<BookPageViewModel>();
            m_navigationService = Container.Current.Resolve<NavigationService>();
        }

        private async Task<BookPageViewModel> StartSelectingBookAsync()
        {
            m_navigationService.SetRootPage();
            m_navigationService.Navigate(typeof(SelectBookView));
            return await m_taskCompletition.Task;
        } 

        private void BookSelected(SelectedPageMessage message)
        {
            Messenger.Default.Unregister(this);
            m_taskCompletition.SetResult(message.BookPage);
        }

        public static async Task<BookPageViewModel> SelectBookAsync()
        {
            var book = new Book();
            return await book.StartSelectingBookAsync();
        } 
    }
}
