using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books.Service;

namespace ITJakub.MobileApps.Client.Books.ViewModel.SelectPage
{
    public class PageTextViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_rtfText;
        private bool m_loading;

        public PageTextViewModel(IDataService dataService)
        {
            m_dataService = dataService;
        }

        public void OpenPage(PageViewModel page)
        {
            RtfText = null;
            Loading = true;
            m_dataService.GetPageAsRtf(Book.Guid, page.PageId, (rtfText, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

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
            get { throw new System.NotImplementedException(); }
        }

        public RelayCommand ZoomInCommand { get; private set; }

        public RelayCommand ZoomOutCommand { get; private set; }
    }
}