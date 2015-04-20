using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books.Service;

namespace ITJakub.MobileApps.Client.Books.ViewModel.SelectPage
{
    public class PagePhotoViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly IErrorService m_errorService;
        private ImageSource m_pagePhoto;
        private bool m_isShowEnabled;
        private bool m_loading;
        private PageViewModel m_currentPage;
        private double m_currentZoom;

        public PagePhotoViewModel(IDataService dataService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_errorService = errorService;

            ZoomInCommand = new RelayCommand(() => CurrentZoom++);
            ZoomOutCommand = new RelayCommand(() => CurrentZoom--);
        }

        public void OpenPagePhoto(PageViewModel page)
        {
            m_currentPage = page;

            PagePhoto = null;
            if (!IsShowEnabled || page == null)
                return;

            Loading = true;
            m_dataService.GetPagePhoto(Book.Guid, page.Name, (image, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowCommunicationWarning();
                    return;
                }

                PagePhoto = image;
            });
        }

        public BookViewModel Book { get; set; }

        public ImageSource PagePhoto
        {
            get { return m_pagePhoto; }
            set
            {
                m_pagePhoto = value;
                RaisePropertyChanged();
            }
        }

        public bool IsShowEnabled
        {
            get { return m_isShowEnabled; }
            set
            {
                m_isShowEnabled = value;
                RaisePropertyChanged();
                OpenPagePhoto(m_currentPage);
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

        public double CurrentZoom
        {
            get { return m_currentZoom; }
            set
            {
                m_currentZoom = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ZoomOutCommand { get; private set; }

        public RelayCommand ZoomInCommand { get; private set; }
    }
}