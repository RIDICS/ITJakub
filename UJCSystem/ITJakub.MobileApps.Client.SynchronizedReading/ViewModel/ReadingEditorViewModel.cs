using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingEditorViewModel : EditorBaseViewModel
    {
        private readonly ReaderDataService m_dataService;

        public ReadingEditorViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
        }
    }
}
