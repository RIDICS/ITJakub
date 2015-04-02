using System;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupPage
{
    public class GroupStateViewModel : FlyoutBaseViewModel
    {
        private readonly Action<GroupStateContract> m_changeStateAction;
        private bool m_isEnabled;

        public GroupStateViewModel(GroupStateContract state, Action<GroupStateContract> changeStateAction)
        {
            m_changeStateAction = changeStateAction;
            GroupState = state;
        }

        protected override void SubmitAction()
        {
            IsFlyoutOpen = false;
            m_changeStateAction(GroupState);
        }

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

        public bool CanChangeBack
        {
            get { return GroupState == GroupStateContract.Paused; }
        }
    }
}