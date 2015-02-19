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
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private ObservableCollection<PageViewModel> m_pageList;
        private BookViewModel m_book;
        private bool m_loading;
        private PageViewModel m_selectedPage;
        private int m_currentPageNumber;
        private ImageSource m_pagePhoto;
        private bool m_loadingPage;
        private bool m_loadingPhoto;
        private bool m_isShowPhotoEnabled;
        private string m_rtfText;

        public SelectPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(navigationService.GoBack);
            SaveCommand = new RelayCommand(Save);

            m_dataService.GetCurrentBook(LoadData);
        }
        
        private void LoadData(BookViewModel bookViewModel)
        {
            Book = bookViewModel;
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

        public ObservableCollection<PageViewModel> PageList
        {
            get { return m_pageList; }
            set
            {
                m_pageList = value;

                RaisePropertyChanged();
                RaisePropertyChanged(() => PageCount);
                RaisePropertyChanged(() => FirstPage);

                MessengerInstance.Send(new PageListMessage(m_pageList));
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

        public PageViewModel SelectedPage
        {
            get { return m_selectedPage; }
            set
            {
                if (value == null || value == m_selectedPage)
                    return;
                
                m_selectedPage = value;

                RaisePropertyChanged();
                CurrentPageNumber = m_pageList.IndexOf(m_selectedPage) + 1;
                OpenPage(m_selectedPage);
            }
        }

        private void OpenPage(PageViewModel page)
        {
            LoadingPage = true;
            m_dataService.GetPageAsRtf(Book.Guid, page.PageId, (rtfText, exception) =>
            {
                LoadingPage = false;
                if (exception != null)
                    return;

                RtfText = rtfText;
            });    

            OpenPagePhoto(page);
        }


        private void OpenPagePhoto(PageViewModel page)
        {
            PagePhoto = null;
            if (!IsShowPhotoEnabled)
                return;

            LoadingPhoto = true;
            m_dataService.GetPagePhoto(Book.Guid, page.PageId, (image, exception) =>
            {
                LoadingPhoto = false;
                if (exception != null)
                    return;

                PagePhoto = image;
            });
        }

        public int CurrentPageNumber
        {
            get { return m_currentPageNumber; }
            set
            {
                m_currentPageNumber = value;
                SelectedPage = m_pageList[CurrentPageNumber - 1];
                RaisePropertyChanged();
            }
        }

        public int PageCount
        {
            get { return m_pageList != null ? m_pageList.Count : 0; }
        }

        public string RtfText
        {
            get { return m_rtfText; }
            set
            {
                m_rtfText = value;
                RaisePropertyChanged();
            }
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

        public bool IsShowPhotoEnabled
        {
            get { return m_isShowPhotoEnabled; }
            set
            {
                m_isShowPhotoEnabled = value;
                OpenPagePhoto(SelectedPage);
            }
        }

        public bool LoadingPage
        {
            get { return m_loadingPage; }
            set
            {
                m_loadingPage = value;
                RaisePropertyChanged();
            }
        }

        public bool LoadingPhoto
        {
            get { return m_loadingPhoto; }
            set
            {
                m_loadingPhoto = value;
                RaisePropertyChanged();
            }
        }

        public int FirstPage
        {
            get { return PageCount == 0 ? 0 : 1; }
        }
        
        private void Save()
        {
            if (SelectedPage == null || LoadingPage)
                return;

            m_navigationService.ResetBackStack();

            var bookDetails = new BookViewModel
            {
                Author = Book.Author,
                Guid = Book.Guid,
                Title = Book.Title,
                Year = Book.Year
            };
            var pageDetails = new BookPageViewModel
            {
                BookInfo = bookDetails,
                PageId = SelectedPage.PageId,
                PagePhoto = PagePhoto,
                RtfText = RtfText
            };
            Messenger.Default.Send(new SelectedPageMessage {BookPage = pageDetails});
        }
    }
}