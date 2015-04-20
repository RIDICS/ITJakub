using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.Service;

namespace ITJakub.MobileApps.Client.Books.ViewModel.SelectPage
{
    public class SelectPageViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private ObservableCollection<PageViewModel> m_pageList;
        private BookViewModel m_book;
        private bool m_loading;
        private PageViewModel m_selectedPage;
        private int m_currentPageNumber;

        public SelectPageViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            PagePhotoViewModel = new PagePhotoViewModel(m_dataService, m_errorService);
            PageTextViewModel = new PageTextViewModel(m_dataService, m_errorService);
            GoBackCommand = new RelayCommand(navigationService.GoBack);
            SelectCommand = new RelayCommand(SubmitSelectedPage);

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
                {
                    m_errorService.ShowCommunicationWarning();
                    return;
                }

                PageList = list;
            });
        }
        
        private void OpenPage(PageViewModel page)
        {
            PageTextViewModel.OpenPage(page);
            PagePhotoViewModel.OpenPagePhoto(page);
        }


        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand SelectCommand { get; private set; }

        public PageTextViewModel PageTextViewModel { get; set; }

        public PagePhotoViewModel PagePhotoViewModel { get; set; }

        public BookViewModel Book
        {
            get { return m_book; }
            set
            {
                m_book = value;
                RaisePropertyChanged();
                PageTextViewModel.Book = value;
                PagePhotoViewModel.Book = value;
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
        
        public int CurrentPageNumber
        {
            get { return m_currentPageNumber; }
            set
            {
                m_currentPageNumber = value;
                SelectedPage = m_pageList[value - 1];
                RaisePropertyChanged();
            }
        }

        public int PageCount
        {
            get { return m_pageList != null ? m_pageList.Count : 0; }
        }
        
        public int FirstPage
        {
            get { return PageCount == 0 ? 0 : 1; }
        }


        private void SubmitSelectedPage()
        {
            if (SelectedPage == null || PageTextViewModel.Loading || PagePhotoViewModel.Loading)
                return;

            m_navigationService.ResetBackStack();

            var bookDetails = new BookViewModel
            {
                Authors = Book.Authors,
                Guid = Book.Guid,
                Title = Book.Title,
                PublishDate = Book.PublishDate
            };
            var pageDetails = new BookPageViewModel
            {
                BookInfo = bookDetails,
                PageName = SelectedPage.Name,
                PagePosition = SelectedPage.Position,
                XmlId = SelectedPage.XmlId,
                PagePhoto = PagePhotoViewModel.PagePhoto,
                RtfText = PageTextViewModel.RtfText
            };
            Messenger.Default.Send(new SelectedPageMessage {BookPage = pageDetails});
        }
    }
}