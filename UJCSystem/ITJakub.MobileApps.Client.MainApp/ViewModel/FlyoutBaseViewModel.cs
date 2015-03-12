using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public abstract class FlyoutBaseViewModel : ViewModelBase
    {
        private bool m_isFlyoutOpen;
        private bool m_inProgress;

        public FlyoutBaseViewModel()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            CancelCommand = new RelayCommand(() => IsFlyoutOpen = false);
        }

        protected abstract void SubmitAction();

        public RelayCommand SubmitCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }
        
        public bool IsFlyoutOpen
        {
            get { return m_isFlyoutOpen; }
            set
            {
                m_isFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }
        
        public bool InProgress
        {
            get { return m_inProgress; }
            set
            {
                m_inProgress = value;
                RaisePropertyChanged();
            }
        }
    }
}