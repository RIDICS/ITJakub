using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        private AnswerState m_answerState = AnswerState.NoAnswer;
        private string m_selectedAnswer;
        private string m_correctAnswer;
        private ObservableCollection<OptionViewModel> m_list;
        private int m_wordPosition;

        public Action<OptionsViewModel> AnswerChangedCallback { get; set; } 

        public int WordPosition
        {
            get { return m_wordPosition; }
            set
            {
                m_wordPosition = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<OptionViewModel> List
        {
            get { return m_list; }
            set
            {
                m_list = value;
                RaisePropertyChanged();
            }
        }

        public string CorrectAnswer
        {
            get { return m_correctAnswer; }
            set
            {
                m_correctAnswer = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedAnswer
        {
            get { return m_selectedAnswer; }
            set
            {
                m_selectedAnswer = value;
                RaisePropertyChanged();

                if (AnswerChangedCallback != null)
                    AnswerChangedCallback(this);
            }
        }

        public void UpdateSelectedAnswer(string newAnswer)
        {
            m_selectedAnswer = newAnswer;
            RaisePropertyChanged(() => SelectedAnswer);
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

    public class OptionViewModel : ViewModelBase
    {
        private string m_word;

        public string Word
        {
            get { return m_word; }
            set
            {
                m_word = value;
                RaisePropertyChanged();
            }
        }
    }
}