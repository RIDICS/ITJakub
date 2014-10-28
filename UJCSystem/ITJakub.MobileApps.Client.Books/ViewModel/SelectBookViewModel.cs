using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Enum;
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
        private bool m_isSearchResult;
        private bool m_searching;
        private ObservableCollection<BookViewModel> m_originalBookList;
        private CategoryContract m_selectedCategory;
        private SortByType m_selectedSortType;

        public SelectBookViewModel(DataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(GoBack);
            BookClickCommand = new RelayCommand<ItemClickEventArgs>(BookClick);
            SearchCommand = new RelayCommand(Search);
            
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

        public bool IsInProgress
        {
            get { return m_loading || m_searching; }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() =>  IsInProgress);
            }
        }

        public bool Searching
        {
            get { return m_searching; }
            set
            {
                m_searching = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() =>  IsInProgress);
            }
        }

        public string SearchQuery { get; set; }

        public RelayCommand SearchCommand { get; private set; }

        public CategoryContract SelectedCategory
        {
            get { return m_selectedCategory; }
            set
            {
                m_selectedCategory = value;
                if (IsSearchResult)
                    Search();
                else
                    LoadData();
            }
        }

        public int DefaultSelectionIndex
        {
            get { return 0; }
        }

        public SortByType SelectedSortType
        {
            get { return m_selectedSortType; }
            set
            {
                m_selectedSortType = value;
                Sort();
            }
        }

        public int FilterFromYear { get; set; }

        public int FilterToYear { get; set; }

        public SearchDestinationContract SearchDestination { get; set; }

        public bool IsSearchResult
        {
            get { return m_isSearchResult; }
            set
            {
                m_isSearchResult = value;
                RaisePropertyChanged();
            }
        }

        private void LoadData()
        {
            Loading = true;
            m_dataService.GetBookList(SelectedCategory, (bookList, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                m_originalBookList = bookList;
                Filter();
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

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            Searching = true;
            m_dataService.SearchForBook(SelectedCategory, SearchDestination, SearchQuery, (bookList, exception) =>
            {
                Searching = false;
                if (exception != null)
                    return;

                IsSearchResult = true;
                RaisePropertyChanged(() => SearchQuery);
                m_originalBookList = bookList;
                Filter();
            });
        }

        private void Sort()
        {
            if (BookList == null)
                return;

            IOrderedEnumerable<BookViewModel> sortedList;
            switch (SelectedSortType)
            {
                case SortByType.Author:
                    sortedList = BookList.OrderBy(book => book.Author);
                    break;
                case SortByType.Name:
                    sortedList = BookList.OrderBy(book => book.Title);
                    break;
                case SortByType.Year:
                    sortedList = BookList.OrderBy(book => book.Year);
                    break;
                default:
                    sortedList = BookList.OrderBy(book => book.Title);
                    break;
            }
            
            BookList = new ObservableCollection<BookViewModel>(sortedList);
        }

        private void Filter()
        {
            // TODO
            BookList = m_originalBookList;
            Sort();
        }
    }
}
