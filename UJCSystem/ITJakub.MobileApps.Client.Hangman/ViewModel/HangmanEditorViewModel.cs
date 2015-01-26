using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanEditorViewModel : EditorBaseViewModel
    {
        private readonly HangmanDataService m_dataService;
        private AnswerViewModel m_selectedAnswer;

        public HangmanEditorViewModel(HangmanDataService dataService)
        {
            m_dataService = dataService;

            AnswerList = new ObservableCollection<AnswerViewModel> {new AnswerViewModel(), new AnswerViewModel()};

            AddAnswerCommand = new RelayCommand(AddAnswer);
            SaveTaskCommand = new RelayCommand(SaveTask);
            DeleteAnswerCommand = new RelayCommand<AnswerViewModel>(DeleteAnswer);
        }

        public RelayCommand AddAnswerCommand { get; private set; }

        public RelayCommand SaveTaskCommand { get; private set; }

        public RelayCommand<AnswerViewModel> DeleteAnswerCommand { get; private set; }

        public ObservableCollection<AnswerViewModel> AnswerList { get; set; }

        public string TaskName { get; set; }

        public AnswerViewModel SelectedAnswer
        {
            get { return m_selectedAnswer; }
            set
            {
                if (m_selectedAnswer != null)
                    m_selectedAnswer.IsSelected = false;

                m_selectedAnswer = value;

                if (m_selectedAnswer != null)
                    m_selectedAnswer.IsSelected = true;

                RaisePropertyChanged();
            }
        }

        private void AddAnswer()
        {
            AnswerList.Add(new AnswerViewModel());
        }

        private void DeleteAnswer(AnswerViewModel answerViewModel)
        {
            AnswerList.Remove(answerViewModel);
        }

        private void SaveTask()
        {
            //TODO
        }
    }
}
