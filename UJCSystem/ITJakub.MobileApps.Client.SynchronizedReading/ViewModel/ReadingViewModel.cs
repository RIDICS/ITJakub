using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingViewModel : ApplicationBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private int m_selectionStart;
        private int m_selectionLength;

        public ReadingViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
        }

        public override void InitializeCommunication()
        {
            m_dataService.StartPollingUpdates((update, exception) =>
            {
                if (exception != null)
                    return;

                SelectionStart = update.SelectionStart;
                SelectionLength = update.SelectionLength;
            });
            SetDataLoaded();
        }

        public override void SetTask(string data)
        {
            //TODO
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
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
    }
}
