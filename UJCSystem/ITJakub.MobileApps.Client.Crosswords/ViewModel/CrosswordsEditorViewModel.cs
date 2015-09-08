using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsEditorViewModel : EditorBaseViewModel
    {
        private readonly ICrosswordsDataService m_dataService;
        private bool m_isSaveFlyoutOpen;
        private bool m_errorTaskNameEmpty;
        private bool m_errorAnswerListEmpty;
        private bool m_errorAnswerColumn;
        private int m_answerColumn;
        private bool m_errorAnswerRowEmpty;
        private bool m_errorTaskDescriptionEmpty;

        public CrosswordsEditorViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;

            AnswerList = new ObservableCollection<EditorItemViewModel> {new EditorItemViewModel(), new EditorItemViewModel()};

            SaveTaskCommand = new RelayCommand(SaveTask);
            CancelCommand = new RelayCommand(() => IsSaveFlyoutOpen = false);
            AddAnswerCommand = new RelayCommand(() => AddRow(true));
            AddSpaceCommand = new RelayCommand(() => AddRow(false));
            DeleteAnswerCommand = new RelayCommand<EditorItemViewModel>(DeleteAnswer);

            ShiftLeftCommand = new RelayCommand(() =>
            {
                if (AnswerColumn > 0)
                    AnswerColumn--;
            });
            ShiftRightCommand = new RelayCommand(() =>
            {
                AnswerColumn++;
            });
        }

        public RelayCommand SaveTaskCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand AddAnswerCommand { get; private set; }

        public RelayCommand AddSpaceCommand { get; private set; }

        public RelayCommand<EditorItemViewModel> DeleteAnswerCommand { get; private set; }

        public RelayCommand ShiftLeftCommand { get; private set; }

        public RelayCommand ShiftRightCommand { get; private set; }

        public ObservableCollection<EditorItemViewModel> AnswerList { get; set; }

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public bool IsSaveFlyoutOpen
        {
            get { return m_isSaveFlyoutOpen; }
            set
            {
                m_isSaveFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorTaskNameEmpty
        {
            get { return m_errorTaskNameEmpty; }
            set
            {
                m_errorTaskNameEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorTaskDescriptionEmpty
        {
            get { return m_errorTaskDescriptionEmpty; }
            set
            {
                m_errorTaskDescriptionEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorAnswerListEmpty
        {
            get { return m_errorAnswerListEmpty; }
            set
            {
                m_errorAnswerListEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorAnswerColumn
        {
            get { return m_errorAnswerColumn; }
            set
            {
                m_errorAnswerColumn = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorAnswerRowEmpty
        {
            get { return m_errorAnswerRowEmpty; }
            set
            {
                m_errorAnswerRowEmpty = value;
                RaisePropertyChanged();
            }
        }

        public int AnswerColumn
        {
            get { return m_answerColumn; }
            set
            {
                m_answerColumn = value;
                RaisePropertyChanged();
            }
        }
        
        public bool IsAnswerListEmpty
        {
            get { return AnswerList.Count == 0; }
        }
        
        private void DeleteAnswer(EditorItemViewModel viewModel)
        {
            AnswerList.Remove(viewModel);
            RaisePropertyChanged(() => IsAnswerListEmpty);
        }

        private void AddRow(bool isAnswer)
        {
            AnswerList.Add(new EditorItemViewModel{IsAnswer = isAnswer});
            RaisePropertyChanged(() => IsAnswerListEmpty);
        }

        private bool IsError()
        {
            var anyError = false;
            ErrorTaskNameEmpty = false;
            ErrorTaskDescriptionEmpty = false;
            ErrorAnswerListEmpty = false;
            ErrorAnswerRowEmpty = false;
            ErrorAnswerColumn = false;

            if (string.IsNullOrWhiteSpace(TaskName))
            {
                ErrorTaskNameEmpty = true;
                anyError = true;
            }

            if (string.IsNullOrWhiteSpace(TaskDescription))
            {
                ErrorTaskDescriptionEmpty = true;
                anyError = true;
            }

            if (IsAnswerListEmpty)
            {
                ErrorAnswerListEmpty = true;
                anyError = true;
            }

            foreach (var editorItemViewModel in AnswerList)
            {
                if (!editorItemViewModel.IsAnswer)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(editorItemViewModel.Answer) || string.IsNullOrWhiteSpace(editorItemViewModel.Label))
                {
                    ErrorAnswerRowEmpty = true;
                    anyError = true;
                    continue;
                }

                var rowShift = editorItemViewModel.Shift;
                var answerLength = editorItemViewModel.Answer.Length;
                if (rowShift > AnswerColumn || rowShift + answerLength <= AnswerColumn)
                {
                    ErrorAnswerColumn = true;
                    anyError = true;
                }
            }

            return anyError;
        }

        private void SaveTask()
        {
            IsSaveFlyoutOpen = false;

            if (IsError())
            {
                return;
            }

            Saving = true;
            m_dataService.SaveTask(TaskName, AnswerList, AnswerColumn, exception =>
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
