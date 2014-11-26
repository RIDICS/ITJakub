using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
        private readonly DispatcherTimer m_delayTimer;
        private ObservableCollection<BookPageViewModel> m_pageList;
        private BookViewModel m_book;
        private bool m_loading;
        private BookPageViewModel m_selectedPage;
        private int m_currentPageNumber;
        private ImageSource m_pagePhoto;
        private bool m_loadingPage;
        private bool m_loadingPhoto;
        private bool m_isShowPhotoEnabled;

        public SelectPageViewModel(DataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_delayTimer = new DispatcherTimer();
            m_delayTimer.Interval = new TimeSpan(0,0,0,0,700);
            m_delayTimer.Tick += DelayedLoadPage;

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

                RaisePropertyChanged();
                RaisePropertyChanged(() => PageCount);
                RaisePropertyChanged(() => FirstPage);

                MessengerInstance.Send(new PageListMessage {PageList = m_pageList});
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
                if (value == null || value == m_selectedPage)
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
                LoadingPage = true;
                m_dataService.GetPageAsRtf(Book.Guid, page.PageId, (rtfText, exception) =>
                {
                    LoadingPage = false;
                    if (exception != null)
                        return;

                    page.RtfText = rtfText;
                    RaisePropertyChanged(() => SelectedPage);
                });    
            }

            OpenPagePhoto(page);
        }

        private void OpenPagePhoto(BookPageViewModel page)
        {
            PagePhoto = null;
            if (!IsShowPhotoEnabled)
                return;

            BitmapImage pagePhoto = page.PagePhoto;
            if (pagePhoto != null)
            {
                PagePhoto = pagePhoto;
            }
            else
            {
                LoadingPhoto = true;
                m_dataService.GetPagePhoto(Book.Guid, page.PageId, (image, exception) =>
                {
                    LoadingPhoto = false;
                    if (exception != null)
                        return;

                    page.PagePhoto = image;
                    OpenPagePhoto(SelectedPage);
                });
            }
        }

        public int CurrentPageNumber
        {
            get { return m_currentPageNumber; }
            set
            {
                m_currentPageNumber = value;
                m_delayTimer.Stop();
                m_delayTimer.Start();
                RaisePropertyChanged();
            }
        }

        public int PageCount
        {
            get { return m_pageList != null ? m_pageList.Count : 0; }
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

        private void DelayedLoadPage(object sender, object o)
        {
            m_delayTimer.Stop();
            var selectedPage = m_pageList[CurrentPageNumber - 1];
            SelectedPage = selectedPage;
        }

        private void Save()
        {
            if (SelectedPage == null || LoadingPage)
                return;

            m_navigationService.GoFromBookSelection();
            SelectedPage.BookInfo = Book;
            Messenger.Default.Send(new SelectedPageMessage
            {
                BookPage = SelectedPage
            });
        }
    }
}