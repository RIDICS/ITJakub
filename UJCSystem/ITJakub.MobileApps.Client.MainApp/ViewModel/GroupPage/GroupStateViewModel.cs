using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupPage
{
    public class GroupStateViewModel : ViewModelBase
    {
        private readonly Action<GroupStateContract> m_changeStateAction;
        private bool m_isEnabled;
        private bool m_isFlyoutOpen;

        public GroupStateViewModel(GroupStateContract state, Action<GroupStateContract> changeStateAction)
        {
            m_changeStateAction = changeStateAction;
            GroupState = state;
            ChangeStateCommand = new RelayCommand(ChangeState);
        }

        public RelayCommand ChangeStateCommand { get; private set; }

        public GroupStateContract GroupState { get; set; }

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                m_isEnabled = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlyoutOpen
        {
            get { return m_isFlyoutOpen; }
            set
            {
                m_isFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool CanChangeBack
        {
            get { return GroupState == GroupStateContract.Paused; }
        }

        private void ChangeState()
        {
            IsFlyoutOpen = false;
            m_changeStateAction(GroupState);
        }
    }
}