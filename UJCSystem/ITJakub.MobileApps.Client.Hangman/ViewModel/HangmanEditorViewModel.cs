using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanEditorViewModel : EditorBaseViewModel
    {
        private readonly HangmanDataService m_dataService;
        private bool m_errorTaskNameEmpty;
        private bool m_errorAnswerListEmpty;
        private bool m_errorSomeAnswerEmpty;
        private bool m_isSaveFlyoutOpen;

        public HangmanEditorViewModel(HangmanDataService dataService)
        {
            m_dataService = dataService;

            AnswerList = new ObservableCollection<AnswerViewModel> {new AnswerViewModel(), new AnswerViewModel()};

            AddAnswerCommand = new RelayCommand(AddAnswer);
            SaveTaskCommand = new RelayCommand(SaveTask);
            CancelCommand= new RelayCommand(() => IsSaveFlyoutOpen = false);
            DeleteAnswerCommand = new RelayCommand<AnswerViewModel>(DeleteAnswer);
        }

        public RelayCommand AddAnswerCommand { get; private set; }

        public RelayCommand SaveTaskCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand<AnswerViewModel> DeleteAnswerCommand { get; private set; }

        public ObservableCollection<AnswerViewModel> AnswerList { get; set; }

        public string TaskName { get; set; }

        public bool IsAnswerListEmpty
        {
            get { return AnswerList.Count == 0; }
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

        public bool ErrorAnswerListEmpty
        {
            get { return m_errorAnswerListEmpty; }
            set
            {
                m_errorAnswerListEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorSomeAnswerEmpty
        {
            get { return m_errorSomeAnswerEmpty; }
            set
            {
                m_errorSomeAnswerEmpty = value;
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
        

        private void AddAnswer()
        {
            AnswerList.Add(new AnswerViewModel());
            RaisePropertyChanged(() => IsAnswerListEmpty);
        }

        private void DeleteAnswer(AnswerViewModel answerViewModel)
        {
            AnswerList.Remove(answerViewModel);
            RaisePropertyChanged(() => IsAnswerListEmpty);
        }

        private void SaveTask()
        {
            IsSaveFlyoutOpen = false;
            if (IsSomeError())
                return;

            Saving = true;
            m_dataService.SaveTask(TaskName, AnswerList, exception =>
            {
                Saving = false;
                if (exception != null)
                    return;

                GoBack();
            });
        }

        private bool IsSomeError()
        {
            ErrorTaskNameEmpty = false;
            ErrorAnswerListEmpty = false;
            ErrorSomeAnswerEmpty = false;

            var anyError = false;

            if (string.IsNullOrWhiteSpace(TaskName))
            {
                ErrorTaskNameEmpty = true;
                anyError = true;
            }

            if (IsAnswerListEmpty)
            {
                ErrorAnswerListEmpty = true;
                anyError = true;
            }
            
            if (AnswerList.Any(answerViewModel => string.IsNullOrWhiteSpace(answerViewModel.Answer)))
            {
                ErrorSomeAnswerEmpty = true;
                anyError = true;
            }

            return anyError;
        }
    }
}
