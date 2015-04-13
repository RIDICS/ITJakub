using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
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
        private ObservableCollection<PageViewModel> m_pageList;
        private bool m_loadingPageList;
        private PageViewModel m_selectedPage;
        private string m_goToPageText;
        private bool m_isPageNotFoundError;
        private bool m_isEnd;

        public ReadingViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;
            m_isPollingStarted = false;

            m_updateSenderTimer = new DispatcherTimer();
            m_updateSenderTimer.Interval = new TimeSpan(0, 0, 0, 0, (int) PollingInterval.VeryFast);
            m_updateSenderTimer.Tick += (sender, o) => SendUpdate();

            TextReaderViewModel = new TextReaderViewModel();
            ImageReaderViewModel = new ImageReaderViewModel();
            m_currentReader = new UserInfo();

            PreviousPageCommand = new RelayCommand(() => JumpToPage(-1));
            NextPageCommand = new RelayCommand(() => JumpToPage(1));
            GoToPageCommand = new RelayCommand(GoToPage);
        }
        
        public override void InitializeCommunication()
        {
            UpdateMode();
            SetDataLoaded();

            LoadingPageList = true;
            m_dataService.GetPageList((pages, exception) =>
            {
                LoadingPageList = false;
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError(GoBack);
                    return;
                }

                PageList = pages;
                UpdateSelectedPage();
            });

            LoadPage();

            m_dataService.StartPollingControlUpdates((model, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }

                CurrentReader = model.ReaderUser;
                LoadPage();
            });
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTask(data);
        }

        public override void EvaluateAndShowResults()
        {
            IsEnd = true;
            CurrentReader = null;
            StopCommunication();
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
                if (m_isPhotoDisplayed == value)
                    return;
                
                m_isPhotoDisplayed = value;
                RaisePropertyChanged();
                LoadPhoto();
            }
        }
        
        public ObservableCollection<PageViewModel> PageList
        {
            get { return m_pageList; }
            set
            {
                m_pageList = value;
                RaisePropertyChanged();
            }
        }

        public bool LoadingPageList
        {
            get { return m_loadingPageList; }
            set
            {
                m_loadingPageList = value;
                RaisePropertyChanged();
            }
        }

        public PageViewModel SelectedPage
        {
            get { return m_selectedPage; }
            set
            {
                if (value == null || value == m_selectedPage)
                    return;

                var lastPage = m_selectedPage;
                m_selectedPage = value;
                RaisePropertyChanged();
                UpdateCurrentPage(lastPage, value);
            }
        }

        public string GoToPageText
        {
            get { return m_goToPageText; }
            set
            {
                m_goToPageText = value;
                RaisePropertyChanged();
            }
        }

        public bool IsPageNotFoundError
        {
            get { return m_isPageNotFoundError; }
            set
            {
                m_isPageNotFoundError = value;
                RaisePropertyChanged();
            }
        }

        public bool IsEnd
        {
            get { return m_isEnd; }
            set
            {
                m_isEnd = value;
                RaisePropertyChanged();
            }
        }

        public TextReaderViewModel TextReaderViewModel { get; set; }

        public ImageReaderViewModel ImageReaderViewModel { get; set; }

        public RelayCommand PreviousPageCommand { get; private set; }

        public RelayCommand NextPageCommand { get; private set; }

        public RelayCommand GoToPageCommand { get; private set; }
        

        private void ProcessPollingUpdate(UpdateViewModel update, Exception exception)
        {
            if (exception != null)
            {
                m_dataService.ErrorService.ShowConnectionWarning();
                return;
            }

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

                if (exception != null)
                    m_dataService.ErrorService.ShowConnectionError();
            });
        }
        
        private void TakeReadControl(ActionViewModel actionViewModel, UserInfo userInfo)
        {
            actionViewModel.IsActionPerformed = true;
            m_dataService.TakeReadControl(exception =>
            {
                actionViewModel.IsActionPerformed = false;

                if (exception != null)
                    m_dataService.ErrorService.ShowConnectionError();
            });
        }

        private void LoadPage()
        {
            IsPageNotFoundError = false;
            TextReaderViewModel.Loading = true;
            m_dataService.GetPageAsRtf((textRtf, exception) =>
            {
                TextReaderViewModel.Loading = false;
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError(GoBack);
                    return;
                }

                TextReaderViewModel.DocumentRtf = textRtf;
                
                if (m_pageList != null)
                    UpdateSelectedPage();
            });

            LoadPhoto();
        }

        private void LoadPhoto()
        {
            if (!IsPhotoDisplayed)
                return;

            ImageReaderViewModel.Loading = true;
            m_dataService.GetPagePhoto((image, exception) =>
            {
                ImageReaderViewModel.Loading = false;
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError();
                    return;
                }

                ImageReaderViewModel.Photo = image;
            });
        }

        private void UpdateSelectedPage()
        {
            m_dataService.GetCurrentPage(pageId =>
            {
                SelectedPage = m_pageList.FirstOrDefault(pageViewModel => pageViewModel.XmlId == pageId);
            });
        }

        private void JumpToPage(int jump)
        {
            var newIndex = m_pageList.IndexOf(SelectedPage) + jump;
            if (newIndex < 0 || newIndex >= m_pageList.Count)
                return;

            SelectedPage = m_pageList[newIndex];
        }

        private void GoToPage()
        {
            IsPageNotFoundError = false;
            if (string.IsNullOrEmpty(GoToPageText))
                return;

            var pageViewModel = m_pageList.FirstOrDefault(page => page.PageName.Equals(GoToPageText, StringComparison.OrdinalIgnoreCase));
            if (pageViewModel == null)
            {
                IsPageNotFoundError = true;
                return;
            }

            SelectedPage = pageViewModel;
        }

        private void UpdateCurrentPage(PageViewModel lastPage, PageViewModel newPage)
        {
            m_dataService.UpdateCurrentPage(newPage.XmlId, exception =>
            {
                if (exception != null)
                {
                    //rollback change current page
                    m_dataService.SetCurrentBook(m_dataService.GetCurrentBookGuid(), lastPage.XmlId);
                    m_selectedPage = lastPage;
                    RaisePropertyChanged(() => SelectedPage);
                    LoadPage();
                }

            });
            LoadPage();
        }
    }
}
