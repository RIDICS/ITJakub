using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Message;

namespace ITJakub.MobileApps.Client.Shared.ViewModel
{
    public class ActionViewModel : ViewModelBase
    {
        private string m_label;
        private bool m_isActionPerformed;

        public ActionViewModel()
        {
            m_isActionPerformed = false;
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

        public bool IsActionPerformed
        {
            get { return m_isActionPerformed; }
            set
            {
                m_isActionPerformed = value;
                RaisePropertyChanged();

                if (!m_isActionPerformed)
                    Messenger.Default.Send(new AppActionFinishedMessage());
            }
        }

        public RelayCommand<UserInfo> Command
        {
            get { return new RelayCommand<UserInfo>(userInfo => Action(this, userInfo)); }
        }

        public Action<ActionViewModel, UserInfo> Action { get; set; } 
    }
}
