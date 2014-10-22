using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.Service;
using ITJakub.MobileApps.Client.Books.View;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    public class SelectBookViewModel : ViewModelBase
    {
        private readonly DataService m_dataService;
        private readonly NavigationService m_navigationService;
        private ObservableCollection<BookViewModel> m_bookList;
        private bool m_noBookFound;
        private bool m_loading;

        public SelectBookViewModel(DataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(GoBack);
            BookClickCommand = new RelayCommand<ItemClickEventArgs>(BookClick);

            LoadData();
        }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand<ItemClickEventArgs> BookClickCommand { get; private set; }
        
        public ObservableCollection<BookViewModel> BookList
        {
            get { return m_bookList; }
            set
            {
                m_bookList = value;
                RaisePropertyChanged();
                NoBookFound = m_bookList.Count == 0;
            }
        }

        public bool NoBookFound
        {
            get { return m_noBookFound; }
            set
            {
                m_noBookFound = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }

        private void LoadData()
        {
            Loading = true;
            m_dataService.GetBookList(CategoryContract.OldGrammar, (bookList, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                BookList = bookList;
            });
        }

        private void GoBack()
        {
            m_navigationService.GoBack();
            Messenger.Default.Send(new SelectedPageMessage());
        }

        private void BookClick(ItemClickEventArgs args)
        {
            var book = args.ClickedItem as BookViewModel;
            if (book == null)
                return;
            
            m_navigationService.Navigate(typeof(SelectPageView));
            Messenger.Default.Send(new SelectedBookMessage {Book = book});
        }
    }
}
