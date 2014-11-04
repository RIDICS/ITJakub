using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
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
        private ImageSource m_pagePhoto;

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

            Loading = true;
            m_dataService.GetPageList(Book.Guid, (list, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                PageList = list;
            });
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
                OpenPage(m_selectedPage);
            }
        }

        private void OpenPage(BookPageViewModel page)
        {
            if (page.RtfText == null)
            {
                m_dataService.GetPageAsRtf(Book.Guid, page.PageId, (rtfText, exception) =>
                {
                    if (exception != null)
                        return;

                    page.RtfText = rtfText;
                });    
            }

            RaisePropertyChanged(() => SelectedPage);
            OpenPagePhoto(page);
        }

        private void OpenPagePhoto(BookPageViewModel page)
        {
            if (!IsShowPhotoEnabled)
            {
                PagePhoto = null;
                return;
            }

            if (page.PagePhoto == null)
            {
                m_dataService.GetPagePhoto(Book.Guid, page.PageId, (image, exception) =>
                {
                    if (exception != null)
                        return;

                    page.PagePhoto = image;
                });
            }

            PagePhoto = page.PagePhoto;
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
            get { return m_pageList != null ? m_pageList.Count : 0; }
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

        public ImageSource PagePhoto
        {
            get { return m_pagePhoto; }
            set
            {
                m_pagePhoto = value;
                RaisePropertyChanged();
            }
        }

        public bool IsShowPhotoEnabled { get; set; }

        private void Save()
        {
            m_navigationService.GoFromBookSelection();
            SelectedPage.BookInfo = Book;
            Messenger.Default.Send(new SelectedPageMessage
            {
                BookPage = SelectedPage
            });
        }
    }
}