using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
        private int m_currentPageNumber;
        private int m_selectedPageIndex;

        public SelectPageViewModel(DataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(navigationService.GoBack);
            SaveCommand = new RelayCommand(Save);
            Messenger.Default.Register<SelectedBookMessage>(this, LoadData);
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
                    RtfText = @"<Paragraph><Span>This is <Bold>mixed content</Bold> with multiple text areas <Italic> and inlines</Italic>.</Span></Paragraph>",
                    PageId = "1L"
                },
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"<Paragraph><Span>This is <Bold>DRUHA</Bold> with multiple text areas <Italic> and inlines</Italic>.</Span></Paragraph>",
                    PageId = "1R"
                },
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"<Paragraph><Span>This is <Bold>TRETI</Bold> with multiple text areas <Italic> and inlines</Italic>.</Span></Paragraph>",
                    PageId = "2L"
                },
                new BookPageViewModel
                {
                    BookInfo = Book,
                    RtfText = @"<Paragraph><Span>This is <Bold>CTVRTA</Bold> with multiple text areas <Italic> and inlines</Italic>.</Span></Paragraph>",
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

                if (m_pageList.Count > 0)
                    SelectedPageIndex = 0;

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
                if (value == null)
                    return;
                
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

        public int SelectedPageIndex
        {
            get { return m_selectedPageIndex; }
            set
            {
                m_selectedPageIndex = value;
                RaisePropertyChanged();
            }
        }

        public string DocumentRtf
        {
            get { return @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text.\par}"; }
        }

        private void Save()
        {
            m_navigationService.GoFromBookSelection();
            Messenger.Default.Send(new SelectedPageMessage
            {
                BookPage = SelectedPage
            });
        }
    }
}