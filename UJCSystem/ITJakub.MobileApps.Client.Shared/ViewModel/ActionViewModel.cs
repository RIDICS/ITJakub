using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Shared.ViewModel
{
    public class ActionViewModel : ViewModelBase
    {
        private string m_label;
        private bool m_isEnabled;

        public ActionViewModel()
        {
            m_isEnabled = true;
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

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                m_isEnabled = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<UserInfo> Command { get; set; } 
    }
}
