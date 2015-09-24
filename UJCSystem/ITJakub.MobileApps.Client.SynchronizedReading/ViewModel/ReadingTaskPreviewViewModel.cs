using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Books.Service.Client;
using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingTaskPreviewViewModel : TaskPreviewBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private string m_bookName;
        private string m_bookAuthor;
        private string m_publishDate;
        private bool m_isShowPhotoEnabled;
        private string m_pageRtfText;
        private ImageSource m_bookPagePhoto;
        private bool m_isPhotoLoadError;
        private bool m_isPageLoadError;
        private bool m_loadingPage;
        private bool m_loadingImage;
        private string m_bookGuid;
        private bool m_isLoaded;

        public ReadingTaskPreviewViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;

            ShowBookReaderCommand = new RelayCommand(ShowReader);
        }
        
        public override void ShowTask(string data)
        {
            IsLoaded = false;
            m_dataService.SetTask(data);
            m_dataService.GetBookInfo((bookInfo, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }

                m_bookGuid = bookInfo.Guid;
                BookName = bookInfo.Title;
                BookAuthor = string.IsNullOrEmpty(bookInfo.Authors) ? "(nezadáno)" : bookInfo.Authors;
                PublishDate = string.IsNullOrEmpty(bookInfo.PublishDate) ? "(nezadáno)" : bookInfo.PublishDate;
                IsLoaded = true;
            });

            LoadingPage = true;
            m_dataService.GetPageAsRtf((pageRtf, exception) =>
            {
                LoadingPage = false;
                if (exception != null)
                {
                    if (exception is NotFoundException)
                    {
                        IsPageLoadError = true;
                        IsShowPhotoEnabled = true;
                    }
                    else
                    {
                        m_dataService.ErrorService.ShowConnectionWarning();
                    }

                    return;
                }

                PageRtfText = pageRtf;
            });
        }

        public string BookName
        {
            get { return m_bookName; }
            set
            {
                m_bookName = value;
                RaisePropertyChanged();
            }
        }

        public string BookAuthor
        {
            get { return m_bookAuthor; }
            set
            {
                m_bookAuthor = value;
                RaisePropertyChanged();
            }
        }

        public string PublishDate
        {
            get { return m_publishDate; }
            set
            {
                m_publishDate = value;
                RaisePropertyChanged();
            }
        }

        public bool IsShowPhotoEnabled
        {
            get { return m_isShowPhotoEnabled; }
            set
            {
                m_isShowPhotoEnabled = value;
                RaisePropertyChanged();
                LoadPagePhoto();
            }
        }
        
        public string PageRtfText
        {
            get { return m_pageRtfText; }
            set
            {
                m_pageRtfText = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource BookPagePhoto
        {
            get { return m_bookPagePhoto; }
            set
            {
                m_bookPagePhoto = value;
                RaisePropertyChanged();
            }
        }

        public bool IsPhotoLoadError
        {
            get { return m_isPhotoLoadError; }
            set
            {
                m_isPhotoLoadError = value;
                RaisePropertyChanged();
            }
        }

        public bool IsPageLoadError
        {
            get { return m_isPageLoadError; }
            set
            {
                m_isPageLoadError = value;
                RaisePropertyChanged();
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

        public bool LoadingImage
        {
            get { return m_loadingImage; }
            set
            {
                m_loadingImage = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLoaded
        {
            get { return m_isLoaded; }
            set
            {
                m_isLoaded = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ShowBookReaderCommand { get; private set; }

        private void LoadPagePhoto()
        {
            IsPhotoLoadError = false;
            if (IsShowPhotoEnabled)
            {
                LoadingImage = true;
                m_dataService.GetPagePhoto((image, exception) =>
                {
                    LoadingImage = false;
                    if (exception != null)
                    {
                        if (exception is NotFoundException)
                            IsPhotoLoadError = true;
                        else
                            m_dataService.ErrorService.ShowConnectionWarning();

                        return;
                    }

                    BookPagePhoto = image;
                });
            }
            else
            {
                BookPagePhoto = null;
            }
        }

        private void ShowReader()
        {
            Book.ShowReader(m_bookGuid);
        }
    }
}