using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Fillwords.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        private AnswerState m_answerState = AnswerState.NoAnswer;
        private string m_selectedAnswer;

        public int WordPosition { get; set; }
        
        public ObservableCollection<OptionViewModel> List { get; set; }

        public string CorrectAnswer { get; set; }

        public string SelectedAnswer
        {
            get { return m_selectedAnswer; }
            set
            {
                m_selectedAnswer = value;
                RaisePropertyChanged();
            }
        }

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

    public class OptionViewModel
    {
        public string Word { get; set; }
    }
}