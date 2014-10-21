using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.Service;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    public class SelectPageViewModel : ViewModelBase
    {
        private readonly DataService m_dataService;
        private readonly NavigationService m_navigationService;
        private ObservableCollection<BookPageViewModel> m_pageList;
        private BookViewModel m_book;
        private bool m_loading;
        private BookPageViewModel m_selectedPage;
        private int m_selectedPageIndex;
        private int m_currentPageNumber;

        public SelectPageViewModel(DataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(navigationService.GoBack);
            SaveCommand = new RelayCommand(Save);
            MessengerInstance.Register<SelectedBookMessage>(this, LoadData);
        }
        
        private void LoadData(SelectedBookMessage message)
        {
            Book = message.Book;
            MessengerInstance.Unregister(this);


            PageList = new ObservableCollection<BookPageViewModel>
            {
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text.\par}",
                    PageId = "1L"
                },
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text, druha strana.\par}",
                    PageId = "1R"
                },
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text, treti strana.\par}",
                    PageId = "2L"
                },
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text, ctvrta strana.\par}",
                    PageId = "2R"
                }
            };
        }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand SaveCommand { get; private set; }

        public BookViewModel Book
        {
            get { return m_book; }
            set
            {
                m_book = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BookPageViewModel> PageList
        {
            get { return m_pageList; }
            set
            {
                m_pageList = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => PageCount);
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

        public BookPageViewModel SelectedPage
        {
            get { return m_selectedPage; }
            set
            {
                m_selectedPage = value;
                RaisePropertyChanged();
                CurrentPageNumber = m_pageList.IndexOf(m_selectedPage) + 1;
            }
        }

        public int CurrentPageNumber
        {
            get { return m_currentPageNumber; }
            set
            {
                m_currentPageNumber = value;
                RaisePropertyChanged();
            }
        }

        public int PageCount
        {
            get { return m_pageList.Count; }
        }

        private void Save()
        {
            m_navigationService.GoFromBookSelection();
            MessengerInstance.Send(new SelectedPageMessage
            {
                BookPage = SelectedPage
            });
        }
    }
}