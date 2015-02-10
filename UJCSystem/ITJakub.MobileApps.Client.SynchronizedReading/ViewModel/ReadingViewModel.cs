using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;
using ITJakub.MobileApps.Client.SynchronizedReading.View.Control;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingViewModel : ApplicationBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private readonly DispatcherTimer m_updateSenderTimer;
        private int m_selectionStart;
        private int m_selectionLength;
        private int m_cursorPosition;
        private bool m_isSelectionModeEnabled;
        private Mode m_currentMode;
        private bool m_isPollingStarted;
        private UpdateViewModel m_lastUpdateViewModel;
        private UserInfo m_currentReader;

        public ReadingViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
            m_isPollingStarted = false;

            m_updateSenderTimer = new DispatcherTimer();
            m_updateSenderTimer.Interval = new TimeSpan(0, 0, 0, 0, (int) PollingInterval.VeryFast);
            m_updateSenderTimer.Tick += (sender, o) => SendUpdate();
        }

        public override void InitializeCommunication()
        {
            UpdateMode();
            SetDataLoaded();

            m_dataService.StartPollingControlUpdates((model, exception) =>
            {
                if (exception != null)
                    return;

                CurrentReader = model.ReaderUser;
            });
        }

        public override void SetTask(string data)
        {
            //TODO
        }

        public override void StopCommunication()
        {
            m_dataService.StopAllPolling();
            m_updateSenderTimer.Stop();
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers
        {
            get
            {
                return new ObservableCollection<ActionViewModel>
                {
                    new ActionViewModel
                    {
                        Label = "Předat čtení",
                        Action = PassReadControl
                    },
                    new ActionViewModel
                    {
                        Label = "Převzít čtení",
                        Action = TakeReadControl
                    }
                };
            }
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

        public UserInfo CurrentReader
        {
            get { return m_currentReader; }
            set
            {
                m_currentReader = value;
                RaisePropertyChanged();
                UpdateMode();
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
            if (m_currentReader != null && m_currentReader.IsMe)
            {
                CurrentMode = IsSelectionModeEnabled ? Mode.Selector : Mode.Pointer;
                m_dataService.StopPollingUpdates();
                m_updateSenderTimer.Start();
                m_isPollingStarted = false;
            }
            else
            {
                CurrentMode = Mode.Reader;
                m_updateSenderTimer.Stop();

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

        private void PassReadControl(ActionViewModel actionViewModel, UserInfo userInfo)
        {
            if (userInfo == null)
                return;

            actionViewModel.IsActionPerformed = true;
            m_dataService.PassControl(userInfo, exception =>
            {
                actionViewModel.IsActionPerformed = false;
            });
        }
        
        private void TakeReadControl(ActionViewModel actionViewModel, UserInfo userInfo)
        {
            actionViewModel.IsActionPerformed = true;
            m_dataService.TakeReadControl(exception =>
            {
                actionViewModel.IsActionPerformed = false;
            });
        }
    }
}
