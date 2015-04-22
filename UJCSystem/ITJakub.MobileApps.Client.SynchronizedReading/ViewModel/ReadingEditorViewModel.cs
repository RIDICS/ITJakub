using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class ReadingEditorViewModel : EditorBaseViewModel
    {
        private readonly ReaderDataService m_dataService;
        private string m_bookName;
        private string m_bookAuthor;
        private string m_publishDate;
        private string m_defaultPageId;
        private string m_pageRtfText;
        private bool m_isSaveFlyoutOpen;
        private bool m_errorTaskNameMissing;
        private bool m_errorBookNotSelected;
        private bool m_isShowPhotoEnabled;
        private ImageSource m_bookPagePhoto;
        private bool m_loadingPhoto;
        private string m_pageName;
        private bool m_isBookSelected;

        public ReadingEditorViewModel(ReaderDataService dataService)
        {
            m_dataService = dataService;

            SelectBookCommand = new RelayCommand(SelectBook);
            SaveTaskCommand = new RelayCommand(SaveTask);
            CancelCommand = new RelayCommand(() => IsSaveFlyoutOpen = false);
        }
        
        public RelayCommand SelectBookCommand { get; private set; }

        public RelayCommand SaveTaskCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        public string TaskName { get; set; }

        public bool IsBookSelected
        {
            get { return m_isBookSelected; }
            set
            {
                m_isBookSelected = value;
                RaisePropertyChanged();
            }
        }

        public string BookName
        {
            get { return m_bookName; }
            set
            {
                m_bookName = value;
                RaisePropertyChanged();
            }
        }

        public string BookAuthor
        {
            get { return m_bookAuthor; }
            set
            {
                m_bookAuthor = value;
                RaisePropertyChanged();
            }
        }

        public string PublishDate
        {
            get { return m_publishDate; }
            set
            {
                m_publishDate = value;
                RaisePropertyChanged();
            }
        }

        public string PageName
        {
            get { return m_pageName; }
            set
            {
                m_pageName = value;
                RaisePropertyChanged();
            }
        }

        public string PageRtfText
        {
            get { return m_pageRtfText; }
            set
            {
                m_pageRtfText = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSaveFlyoutOpen
        {
            get { return m_isSaveFlyoutOpen; }
            set
            {
                m_isSaveFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorTaskNameMissing
        {
            get { return m_errorTaskNameMissing; }
            set
            {
                m_errorTaskNameMissing = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorBookNotSelected
        {
            get { return m_errorBookNotSelected; }
            set
            {
                m_errorBookNotSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool IsShowPhotoEnabled
        {
            get { return m_isShowPhotoEnabled; }
            set
            {
                m_isShowPhotoEnabled = value;
                RaisePropertyChanged();
                LoadPhoto();
            }
        }

        public ImageSource BookPagePhoto
        {
            get { return m_bookPagePhoto; }
            set
            {
                m_bookPagePhoto = value;
                RaisePropertyChanged();
            }
        }

        public bool LoadingPhoto
        {
            get { return m_loadingPhoto; }
            set
            {
                m_loadingPhoto = value;
                RaisePropertyChanged();
            }
        }
        
        private void HideErrors()
        {
            ErrorBookNotSelected = false;
            ErrorTaskNameMissing = false;
        }

        private async void SelectBook()
        {
            HideErrors();
            var book = await Book.SelectBookAsync();
            if (book == null)
                return;

            IsBookSelected = true;
            m_dataService.SetCurrentBook(book.BookInfo.Guid, book.XmlId);
            BookAuthor = book.BookInfo.Authors;
            BookName = book.BookInfo.Title;
            PublishDate = book.BookInfo.PublishDate;
            PageName = book.PageName;
            m_defaultPageId = book.XmlId;
            PageRtfText = book.RtfText;
            BookPagePhoto = book.PagePhoto;

            IsShowPhotoEnabled = BookPagePhoto != null;
        }
        
        private void LoadPhoto()
        {
            if (IsShowPhotoEnabled)
            {
                if (BookPagePhoto != null)
                    return;

                LoadingPhoto = true;
                m_dataService.GetPagePhoto((image, exception) =>
                {
                    LoadingPhoto = false;
                    if (exception != null)
                    {
                        m_dataService.ErrorService.ShowConnectionError(() => IsShowPhotoEnabled = false);
                        return;
                    }

                    BookPagePhoto = image;
                });
            }
            else
            {
                BookPagePhoto = null;
            }
        }

        private bool IsError()
        {
            HideErrors();
            var isAnyError = false;

            if (string.IsNullOrWhiteSpace(TaskName))
            {
                ErrorTaskNameMissing = true;
                isAnyError = true;
            }

            if (m_defaultPageId == null)
            {
                ErrorBookNotSelected = true;
                isAnyError = true;
            }

            return isAnyError;
        }

        private void SaveTask()
        {
            IsSaveFlyoutOpen = false;

            if (IsError())
                return;

            Saving = true;
            m_dataService.CreateTask(TaskName, m_defaultPageId, exception =>
            {
                Saving = false;
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError();
                    return;
                }

                GoBack();
            });
        }
    }
}
