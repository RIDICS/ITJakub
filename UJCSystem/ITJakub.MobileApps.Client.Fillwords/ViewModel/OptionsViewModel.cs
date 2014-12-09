using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        private AnswerState m_answerState = AnswerState.NoAnswer;

        public int WordPosition { get; set; }
        
        public ObservableCollection<OptionViewModel> List { get; set; }

        public string CorrectAnswer { get; set; }

        public string SelectedAnswer { get; set; }

        public AnswerState AnswerState
        {
            get { return m_answerState; }
            set
            {
                m_answerState = value;
                RaisePropertyChanged();
            }
        }
    }

    public enum AnswerState
    {
        NoAnswer,
        Correct,
        Incorrect
    }

    public class OptionViewModel
    {
        public string Word { get; set; }
    }
}