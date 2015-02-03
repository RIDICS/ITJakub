using System;
using Windows.UI.Xaml;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;
using ITJakub.MobileApps.Client.SynchronizedReading.View.Control;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingViewModel : ApplicationBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private readonly DispatcherTimer m_timer;
        private int m_selectionStart;
        private int m_selectionLength;
        private int m_cursorPosition;
        private bool m_isTeacherModeEnabled;
        private bool m_isSelectionModeEnabled;
        private Mode m_currentMode;
        private bool m_isPollingStarted;
        private UpdateViewModel m_lastUpdateViewModel;

        public ReadingViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
            m_isPollingStarted = false;

            m_timer = new DispatcherTimer();
            m_timer.Interval = new TimeSpan(0, 0, 0, 0, (int) PollingInterval.VeryFast);
            m_timer.Tick += (sender, o) => SendUpdate();
        }

        public override void InitializeCommunication()
        {
            UpdateMode();
            SetDataLoaded();
        }

        public override void SetTask(string data)
        {
            //TODO
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
            m_timer.Stop();
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

        public bool IsTeacherModeEnabled
        {
            get { return m_isTeacherModeEnabled; }
            set
            {
                m_isTeacherModeEnabled = value;
                RaisePropertyChanged();
                UpdateMode();
            }
        }

        public bool IsSelectionModeEnabled
        {
            get { return m_isSelectionModeEnabled; }
            set
            {
                m_isSelectionModeEnabled = value;
                RaisePropertyChanged();
                UpdateMode();
            }
        }

        public Mode CurrentMode
        {
            get { return m_currentMode; }
            set
            {
                m_currentMode = value;
                RaisePropertyChanged();
            }
        }

        private void ProcessPollingUpdate(UpdateViewModel update, Exception exception)
        {
            if (exception != null)
                return;

            SelectionStart = update.SelectionStart;
            SelectionLength = update.SelectionLength;
            CursorPosition = update.CursorPosition;
        }

        private void UpdateMode()
        {
            if (IsTeacherModeEnabled)
            {
                CurrentMode = IsSelectionModeEnabled ? Mode.Selector : Mode.Pointer;
                m_dataService.StopPolling();
                m_timer.Start();
                m_isPollingStarted = false;
            }
            else
            {
                CurrentMode = Mode.Reader;
                m_timer.Stop();

                if (m_isPollingStarted)
                    return;

                m_dataService.StartPollingUpdates(ProcessPollingUpdate);
                m_isPollingStarted = true;
            }
        }

        private void SendUpdate()
        {
            if (m_currentMode == Mode.Reader)
                return;

            var updateViewModel = new UpdateViewModel
            {
                SelectionStart = m_selectionStart,
                SelectionLength = m_selectionLength,
                CursorPosition = m_cursorPosition
            };

            if (updateViewModel.Equals(m_lastUpdateViewModel))
                return;

            m_lastUpdateViewModel = updateViewModel;
            m_dataService.SendUpdate(updateViewModel, exception => { });
        }
    }
}
