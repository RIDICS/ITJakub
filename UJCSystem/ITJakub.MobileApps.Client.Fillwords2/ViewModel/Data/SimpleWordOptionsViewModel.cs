using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data
{
    public class SimpleWordOptionsViewModel : ViewModelBase
    {
        private ObservableCollection<LetterOptionViewModel> m_options;
        private int m_wordPosition;
        private string m_correctAnswer;
        private AnswerState m_answerState;
        private string m_selectedAnswer;

        public SimpleWordOptionsViewModel()
        {
            Options = new ObservableCollection<LetterOptionViewModel>();
        }

        public Action<SimpleWordOptionsViewModel> AnswerChangedCallback { get; set; }

        public int WordPosition
        {
            get { return m_wordPosition; }
            set
            {
                m_wordPosition = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<LetterOptionViewModel> Options
        {
            get { return m_options; }
            set
            {
                m_options = value;
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

        public AnswerState AnswerState
        {
            get { return m_answerState; }
            set
            {
                m_answerState = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedAnswer
        {
            get { return m_selectedAnswer; }
            set
            {
                if (m_selectedAnswer == value)
                    return;

                m_selectedAnswer = value;
                RaisePropertyChanged();
            }
        }

        public void UpdateSelectedAnswer(IList<ConcreteAnswerViewModel> answers)
        {
            foreach (var concreteAnswerViewModel in answers)
            {
                var concreteOption = Options.SingleOrDefault(x => x.StartPosition == concreteAnswerViewModel.StartPosition);
                if (concreteOption == null)
                    return;

                concreteOption.SelectedAnswer = concreteAnswerViewModel.Answer;
            }
            UpdateCompleteSelectedAnswer();
        }

        public void UpdateCompleteSelectedAnswer()
        {
            var stringBuilder = new StringBuilder();

            int startPosition = 0;
            int endPosition;
            var correctAnswer = CorrectAnswer;

            foreach (var letterOptionViewModel in Options)
            {
                var selectedAnswer = string.IsNullOrEmpty(letterOptionViewModel.SelectedAnswer)
                    ? "_"
                    : letterOptionViewModel.SelectedAnswer;

                endPosition = letterOptionViewModel.StartPosition;

                stringBuilder.Append(correctAnswer.Substring(startPosition, endPosition - startPosition));
                stringBuilder.Append(selectedAnswer);

                startPosition = letterOptionViewModel.EndPosition;
            }

            endPosition = correctAnswer.Length;
            stringBuilder.Append(correctAnswer.Substring(startPosition, endPosition - startPosition));

            var resultText = stringBuilder.ToString();
            SelectedAnswer = resultText;
        }

        public void SubmitAnswer()
        {
            if (AnswerChangedCallback != null)
                AnswerChangedCallback(this);
        }
    }
}