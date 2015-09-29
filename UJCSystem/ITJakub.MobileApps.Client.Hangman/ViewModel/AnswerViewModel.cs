using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class AnswerViewModel : ViewModelBase
    {
        private string m_answer;
        private string m_hint;

        public string Answer
        {
            get { return m_answer; }
            set
            {
                m_answer = value;
                RaisePropertyChanged();
            }
        }

        public string Hint
        {
            get { return m_hint; }
            set
            {
                m_hint = value;
                RaisePropertyChanged();
            }
        }
    }
}
