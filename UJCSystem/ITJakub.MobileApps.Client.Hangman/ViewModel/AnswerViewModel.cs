using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class AnswerViewModel : ViewModelBase
    {
        private string m_answer;

        public string Answer
        {
            get { return m_answer; }
            set
            {
                m_answer = value;
                RaisePropertyChanged();
            }
        }
    }
}
