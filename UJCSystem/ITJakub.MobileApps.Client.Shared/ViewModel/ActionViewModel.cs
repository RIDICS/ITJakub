using System;
using GalaSoft.MvvmLight;
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

        public Action<UserInfo> Action { get; set; }
    }
}
