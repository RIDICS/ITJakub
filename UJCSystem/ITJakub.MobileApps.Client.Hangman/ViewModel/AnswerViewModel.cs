using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class AnswerViewModel : ViewModelBase
    {
        private string m_answer;
        private bool m_isSelected;

        public string Answer
        {
            get { return m_answer; }
            set
            {
                m_answer = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                RaisePropertyChanged();
            }
        }
    }
}
