using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class UserResultViewModel : ViewModelBase
    {
        private int m_correctAnswers;
        private int m_totalAnswers;
        private ObservableCollection<ResultAnswerViewModel> m_answers;
        private bool m_isTaskSubmited;

        public UserInfo UserInfo { get; set; }

        public double PercentageSuccess
        {
            get { return (double) CorrectAnswers/TotalAnswers*100; }
        }

        public int CorrectAnswers
        {
            get { return m_correctAnswers; }
            set
            {
                m_correctAnswers = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => PercentageSuccess);
            }
        }

        public int TotalAnswers
        {
            get { return m_totalAnswers; }
            set
            {
                m_totalAnswers = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => PercentageSuccess);
            }
        }

        public ObservableCollection<ResultAnswerViewModel> Answers
        {
            get { return m_answers; }
            set
            {
                m_answers = value;
                RaisePropertyChanged();
            }
        }

        public bool IsTaskSubmited
        {
            get { return m_isTaskSubmited; }
            set
            {
                m_isTaskSubmited = value;
                RaisePropertyChanged();
            }
        }
    }

    public class ResultAnswerViewModel
    {
        public string Answer { get; set; }

        public bool IsCorrect { get; set; }
    }
}