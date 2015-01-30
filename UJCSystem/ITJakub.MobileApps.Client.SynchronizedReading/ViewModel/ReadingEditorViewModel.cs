using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingEditorViewModel : EditorBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private int m_selectionStart;
        private int m_selectionLength;
        private int m_cursorPosition;

        public ReadingEditorViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
            SelectionChangedCommand = new RelayCommand(SendUpdate);
        }

        public int SelectionStart
        {
            get { return m_selectionStart; }
            set
            {
                m_selectionStart = value;
                RaisePropertyChanged();
            }
        }

        public int SelectionLength
        {
            get { return m_selectionLength; }
            set
            {
                m_selectionLength = value;
                RaisePropertyChanged();
            }
        }

        public int CursorPosition
        {
            get { return m_cursorPosition; }
            set
            {
                m_cursorPosition = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SelectionChangedCommand { get; private set; }

        //TODO this method is for testing
        private void SendUpdate()
        {
            if (m_selectionLength == 0)
                return;
            
            var updateViewModel = new UpdateViewModel
            {
                SelectionStart = m_selectionStart,
                SelectionLength = m_selectionLength
            };

            m_dataService.SendUpdate(updateViewModel, exception => {});
        }
    }
}
