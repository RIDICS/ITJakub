using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class EditorItemViewModel : ViewModelBase
    {
        private int m_shift;
        private bool m_isAnswer;
        private string m_answer;
        private string m_label;
        private int m_shiftBackup;

        public EditorItemViewModel()
        {
            Shift = 0;
            m_shiftBackup = Shift;
            IsAnswer = true;

            ShiftLeftCommand = new RelayCommand(() =>
            {
                if (Shift > 0)
                    Shift--;
            });
            ShiftRightCommand = new RelayCommand(() =>
            {
                Shift++;
            });
        }

        public RelayCommand ShiftLeftCommand { get; private set; }

        public RelayCommand ShiftRightCommand { get; private set; }

        public string Answer
        {
            get { return m_answer; }
            set
            {
                m_answer = value;
                RaisePropertyChanged();
            }
        }

        public string Label
        {
            get { return m_label; }
            set
            {
                m_label = value;
                RaisePropertyChanged();
            }
        }
        
        public int Shift
        {
            get { return m_shift; }
            set
            {
                m_shift = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAnswer
        {
            get { return m_isAnswer; }
            set
            {
                m_isAnswer = value;
                RaisePropertyChanged();

                if (m_isAnswer)
                {
                    Shift = m_shiftBackup;
                }
                else
                {
                    m_shiftBackup = Shift;
                    Shift = 0;
                }
            }
        }
    }
}