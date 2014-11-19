using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class EditorViewModel : EditorBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private string m_selectedText;
        private Dictionary<int, OptionsViewModel> m_wordOptionsList;
        private OptionsViewModel m_selectedOption;
        private string m_newOption;
        private bool m_isEditorFlyoutOpen;
        private bool m_isTextEditingEnabled;
        private string m_bookName;
        private string m_bookAuthor;
        private int m_bookYear;
        private string m_bookRtfContent;
        private BitmapImage m_bookPagePhoto;
        private bool m_isShowPhotoEnabled;
        private bool m_errorNameMissing;
        private bool m_errorPageEmpty;
        private bool m_errorOptionsMissing;
        private bool m_isSaveFlyoutOpen;
        private bool m_loadingPhoto;

        public EditorViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
            IsTextEditingEnabled = false;

            WordOptionsList = new Dictionary<int, OptionsViewModel>();
            AddNewOptionCommand = new RelayCommand(AddNewOption);
            SaveOptionsCommand = new RelayCommand(SaveOptions);
            SelectBookCommand = new RelayCommand(SelectBook);
            SaveTaskCommand = new RelayCommand(SaveTask);
        }

        #region Properties

        public string SelectedText
        {
            get { return m_selectedText; }
            set
            {
                m_selectedText = value;
                RaisePropertyChanged();
            }
        }

        public Dictionary<int, OptionsViewModel> WordOptionsList
        {
            get { return m_wordOptionsList; }
            set
            {
                m_wordOptionsList = value;
                RaisePropertyChanged();
            }
        }

        public OptionsViewModel SelectedOption
        {
            get { return m_selectedOption; }
            set
            {
                m_selectedOption = value;
                NewOption = string.Empty;
                RaisePropertyChanged();
            }
        }

        public string NewOption
        {
            get { return m_newOption; }
            set
            {
                m_newOption = value;
                RaisePropertyChanged();
            }
        }

        public bool IsEditorFlyoutOpen
        {
            get { return m_isEditorFlyoutOpen; }
            set
            {
                m_isEditorFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool IsTextEditingEnabled
        {
            get { return m_isTextEditingEnabled; }
            set
            {
                m_isTextEditingEnabled = value;
                if (value)
                    WordOptionsList.Clear();

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

        public int BookYear
        {
            get { return m_bookYear; }
            set
            {
                m_bookYear = value;
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

        public BitmapImage BookPagePhoto
        {
            get { return m_bookPagePhoto; }
            set
            {
                m_bookPagePhoto = value;
                RaisePropertyChanged();
            }
        }

        public bool IsShowPhotoEnabled
        {
            get { return m_isShowPhotoEnabled; }
            set
            {
                m_isShowPhotoEnabled = value;
                LoadPagePhoto();
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

        public bool ErrorNameMissing
        {
            get { return m_errorNameMissing; }
            set
            {
                m_errorNameMissing = value;
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

        #endregion

        #region Commands

        public RelayCommand AddNewOptionCommand { get; private set; }

        public RelayCommand SaveOptionsCommand { get; private set; }

        public RelayCommand SelectBookCommand { get; private set; }

        public RelayCommand SaveTaskCommand { get; private set; }

        #endregion

        private void AddNewOption()
        {
            if (NewOption == string.Empty)
                return;

            var newOptionViewModel = new OptionViewModel
            {
                Word = NewOption,
            };
            newOptionViewModel.DeleteCommand = new RelayCommand(() => SelectedOption.List.Remove(newOptionViewModel));

            SelectedOption.List.Add(newOptionViewModel);

            NewOption = string.Empty;
        }

        private void SaveOptions()
        {
            var key = SelectedOption.WordPosition;

            if (SelectedOption.List.Count == 0)
                WordOptionsList.Remove(key);
            else
                WordOptionsList[key] = SelectedOption;

            IsEditorFlyoutOpen = false;
        }

        private async void SelectBook()
        {
            HideErrors();

            var book = await Book.SelectBookAsync();
            if (book == null)
                return;

            BookAuthor = book.BookInfo.Author;
            BookName = book.BookInfo.Title;
            BookYear = book.BookInfo.Year;
            BookRtfContent = book.RtfText;
            BookPagePhoto = book.PagePhoto; // may be null

            LoadPagePhoto();
        }

        private void LoadPagePhoto()
        {
            if (IsShowPhotoEnabled && BookPagePhoto == null)
            {
                //TODO load photo
            }
        }

        private void HideErrors()
        {
            ErrorNameMissing = false;
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

            if (string.IsNullOrEmpty(BookRtfContent))
            {
                ErrorPageEmpty = true;
                return true;
            }

            if (WordOptionsList.Count == 0)
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
            
            m_dataService.CreateTask(TaskName, BookRtfContent, WordOptionsList.Values.ToList(), exception =>
            {
                if (exception != null)
                    return;

                //TODO complete task creation
            });
        }
    }
}
