using System;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;
using ITJakub.MobileApps.Client.SynchronizedReading.View.Control;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingViewModel : ApplicationBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private int m_selectionStart;
        private int m_selectionLength;
        private int m_cursorPosition;
        private bool m_isTeacherModeEnabled;
        private bool m_isSelectionModeEnabled;
        private Mode m_currentMode;
        private bool m_isPollingStarted;

        public ReadingViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
            m_isPollingStarted = false;
            SelectionChangedCommand = new RelayCommand(SendUpdate);
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
                SendUpdate();
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

        public RelayCommand SelectionChangedCommand { get; private set; }

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
                m_isPollingStarted = false;
            }
            else
            {
                CurrentMode = Mode.Reader;
                
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

            m_dataService.SendUpdate(updateViewModel, exception => { });
        }
    }
}
