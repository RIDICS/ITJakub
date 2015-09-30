using System.Linq;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Fillwords2.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class FillwordsEditorViewModel : EditorBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private bool m_isTextEditingEnabled;
        private string m_bookName;
        private string m_bookAuthor;
        private string m_publishDate;
        private string m_bookRtfContent;
        private bool m_errorNameMissing;
        private bool m_errorPageEmpty;
        private bool m_errorOptionsMissing;
        private bool m_isSaveFlyoutOpen;
        private bool m_errorDescriptionMissing;

        public FillwordsEditorViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;

            OptionsEditorViewModel = new OptionsEditorViewModel();
            IsTextEditingEnabled = true;

            SelectBookCommand = new RelayCommand(SelectBook);
            SaveTaskCommand = new RelayCommand(SaveTask);
            CancelCommand = new RelayCommand(() => IsSaveFlyoutOpen = false);
        }

        #region Properties

        public bool IsTextEditingEnabled
        {
            get { return m_isTextEditingEnabled; }
            set
            {
                m_isTextEditingEnabled = value;
                if (value)
                    OptionsEditorViewModel.Reset();

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

        public string BookRtfContent
        {
            get { return m_bookRtfContent; }
            set
            {
                m_bookRtfContent = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorNameMissing
        {
            get { return m_errorNameMissing; }
            set
            {
                m_errorNameMissing = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorDescriptionMissing
        {
            get { return m_errorDescriptionMissing; }
            set
            {
                m_errorDescriptionMissing = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorPageEmpty
        {
            get { return m_errorPageEmpty; }
            set
            {
                m_errorPageEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorOptionsMissing
        {
            get { return m_errorOptionsMissing; }
            set
            {
                m_errorOptionsMissing = value;
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

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public OptionsEditorViewModel OptionsEditorViewModel { get; private set; }

        public RelayCommand SelectBookCommand { get; private set; }

        public RelayCommand SaveTaskCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        #endregion


        private async void SelectBook()
        {
            HideErrors();

            var book = await Book.SelectBookAsync();
            if (book == null)
                return;

            IsTextEditingEnabled = false;
            BookName = book.BookInfo.Title;
            BookAuthor = string.IsNullOrEmpty(book.BookInfo.Authors) ? "(nezadáno)" : book.BookInfo.Authors;
            PublishDate = string.IsNullOrEmpty(book.BookInfo.PublishDate) ? "(nezadáno)" : book.BookInfo.PublishDate;
            BookRtfContent = book.RtfText;
        }

        private void HideErrors()
        {
            ErrorNameMissing = false;
            ErrorDescriptionMissing = false;
            ErrorPageEmpty = false;
            ErrorOptionsMissing = false;
        }

        private bool IsSaveError()
        {
            HideErrors();

            if (string.IsNullOrEmpty(TaskName))
            {
                ErrorNameMissing = true;
                return true;
            }

            if (string.IsNullOrEmpty(TaskDescription))
            {
                ErrorDescriptionMissing = true;
                return true;
            }

            if (string.IsNullOrEmpty(BookRtfContent))
            {
                ErrorPageEmpty = true;
                return true;
            }

            if (OptionsEditorViewModel.WordOptionsList.Count == 0)
            {
                ErrorOptionsMissing = true;
                return true;
            }

            return false;
        }

        private void SaveTask()
        {
            if (IsSaveError())
            {
                IsSaveFlyoutOpen = false;
                return;
            }

            Saving = true;
            //m_dataService.CreateTask(TaskName, TaskDescription, BookRtfContent, OptionsEditorViewModel.WordOptionsList.Values.ToList(), exception =>
            //{
            //    Saving = false;
            //    if (exception != null)
            //    {
            //        m_dataService.ErrorService.ShowConnectionError();
            //        return;
            //    }

            //    GoBack();
            //});
        }
    }
}