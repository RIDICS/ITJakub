using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;
using ITJakub.MobileApps.Client.SynchronizedReading.View.Control;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel.Reading
{
    public class ReadingViewModel : ApplicationBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private readonly DispatcherTimer m_updateSenderTimer;
        private bool m_isPollingStarted;
        private UpdateViewModel m_lastUpdateViewModel;
        private UserInfo m_currentReader;
        private bool m_isPhotoDisplayed;
        private bool m_isSelectionModeEnabled;

        public ReadingViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
            m_isPollingStarted = false;

            m_updateSenderTimer = new DispatcherTimer();
            m_updateSenderTimer.Interval = new TimeSpan(0, 0, 0, 0, (int) PollingInterval.VeryFast);
            m_updateSenderTimer.Tick += (sender, o) => SendUpdate();

            TextReaderViewModel = new TextReaderViewModel();
            ImageReaderViewModel = new ImageReaderViewModel();
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

        private void UpdateMode()
        {
            if (m_currentReader != null && m_currentReader.IsMe)
            {
                TextReaderViewModel.CurrentMode = m_isSelectionModeEnabled ? ReaderRichEditBox.Modes.Selector : ReaderRichEditBox.Modes.Pointer;
                ImageReaderViewModel.CurrentMode = ReaderImage.Modes.Pointer;

                m_dataService.StopPollingUpdates();
                m_updateSenderTimer.Start();
                m_isPollingStarted = false;
            }
            else
            {
                TextReaderViewModel.CurrentMode = ReaderRichEditBox.Modes.Reader;
                ImageReaderViewModel.CurrentMode = ReaderImage.Modes.Reader;
                m_updateSenderTimer.Stop();

                if (m_isPollingStarted)
                    return;

                m_dataService.StartPollingUpdates(ProcessPollingUpdate);
                m_isPollingStarted = true;
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

        public bool IsPhotoDisplayed
        {
            get { return m_isPhotoDisplayed; }
            set
            {
                m_isPhotoDisplayed = value;
                RaisePropertyChanged();
            }
        }

        public TextReaderViewModel TextReaderViewModel { get; set; }

        public ImageReaderViewModel ImageReaderViewModel { get; set; }


        private void ProcessPollingUpdate(UpdateViewModel update, Exception exception)
        {
            if (exception != null)
                return;

            TextReaderViewModel.SelectionStart = update.SelectionStart;
            TextReaderViewModel.SelectionLength = update.SelectionLength;
            TextReaderViewModel.CursorPosition = update.CursorPosition;

            if (update.ContainsImageUpdate)
            {
                ImageReaderViewModel.PointerPositionX = update.ImageCursorPositionX;
                ImageReaderViewModel.PointerPositionY = update.ImageCursorPositionY;
                IsPhotoDisplayed = true;
            }
        }


        private void SendUpdate()
        {
            if (TextReaderViewModel.CurrentMode == ReaderRichEditBox.Modes.Reader)
                return;

            var updateViewModel = new UpdateViewModel
            {
                SelectionStart = TextReaderViewModel.SelectionStart,
                SelectionLength = TextReaderViewModel.SelectionLength,
                CursorPosition = TextReaderViewModel.CursorPosition,
                ContainsImageUpdate = IsPhotoDisplayed
            };

            if (IsPhotoDisplayed)
            {
                updateViewModel.ImageCursorPositionX = ImageReaderViewModel.PointerPositionX;
                updateViewModel.ImageCursorPositionY = ImageReaderViewModel.PointerPositionY;
            }

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
