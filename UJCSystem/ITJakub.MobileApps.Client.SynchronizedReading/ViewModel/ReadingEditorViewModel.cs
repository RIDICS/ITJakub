using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingEditorViewModel : EditorBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private int m_selectionStart;
        private int m_selectionLength;

        public ReadingEditorViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
        }

        public int SelectionStart
        {
            get { return m_selectionStart; }
            set
            {
                m_selectionStart = value;
                RaisePropertyChanged();
                SendUpdate();
            }
        }

        public int SelectionLength
        {
            get { return m_selectionLength; }
            set
            {
                m_selectionLength = value;
                RaisePropertyChanged();
                SendUpdate();
            }
        }

        //TODO this method is for testing
        private void SendUpdate()
        {
            var updateViewModel = new UpdateViewModel
            {
                SelectionStart = m_selectionStart,
                SelectionLength = m_selectionLength
            };

            m_dataService.SendUpdate(updateViewModel, exception => {});
        }
    }
}
