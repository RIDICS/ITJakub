using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Fillwords.DataService;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class EditorViewModel : ViewModelBase
    {
        private readonly FillwordsDataService m_dataService;

        public EditorViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
        }
    }
}
