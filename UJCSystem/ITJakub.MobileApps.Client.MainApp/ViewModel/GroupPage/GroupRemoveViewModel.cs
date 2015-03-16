using System;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupPage
{
    public class GroupRemoveViewModel : FlyoutBaseViewModel
    {
        private readonly Action m_removeGroupAction;

        public GroupRemoveViewModel(Action removeGroupAction)
        {
            m_removeGroupAction = removeGroupAction;
        }

        protected override void SubmitAction()
        {
            m_removeGroupAction();
        }
    }
}