using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class EditorItemViewModel : ViewModelBase
    {
        private int m_shift;

        public EditorItemViewModel()
        {
            Shift = 0;

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

        public string Answer { get; set; }

        public string Label { get; set; }

        public RelayCommand ShiftLeftCommand { get; private set; }

        public RelayCommand ShiftRightCommand { get; private set; }

        public int Shift
        {
            get { return m_shift; }
            set
            {
                m_shift = value;
                RaisePropertyChanged();
            }
        }
    }
}