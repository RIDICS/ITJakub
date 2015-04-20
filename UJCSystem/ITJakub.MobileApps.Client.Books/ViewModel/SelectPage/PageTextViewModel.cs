using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books.Service;

namespace ITJakub.MobileApps.Client.Books.ViewModel.SelectPage
{
    public class PageTextViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly IErrorService m_errorService;
        private string m_rtfText;
        private bool m_loading;
        private double m_currentZoom;

        public PageTextViewModel(IDataService dataService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_errorService = errorService;

            ZoomInCommand = new RelayCommand(() => CurrentZoom++);
            ZoomOutCommand = new RelayCommand(() => CurrentZoom--);
        }

        public void OpenPage(PageViewModel page)
        {
            RtfText = null;
            Loading = true;
            m_dataService.GetPageAsRtf(Book.Guid, page.XmlId, (rtfText, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowCommunicationWarning();
                    return;
                }

                RtfText = rtfText;
            });
        }
        
        public BookViewModel Book { get; set; }

        public string RtfText
        {
            get { return m_rtfText; }
            set
            {
                m_rtfText = value;
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

        public double CurrentZoom
        {
            get { return m_currentZoom; }
            set
            {
                m_currentZoom = value; 
                RaisePropertyChanged();
            }
        }

        public RelayCommand ZoomInCommand { get; private set; }

        public RelayCommand ZoomOutCommand { get; private set; }
    }
}