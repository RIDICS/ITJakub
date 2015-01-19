using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class DeleteGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly Action m_refreshAction;
        private bool m_inProgress;
        private bool m_showError;
        private GroupInfoViewModel m_selectedGroup;

        public DeleteGroupViewModel(IDataService dataService, Action refreshAction)
        {
            m_dataService = dataService;
            m_refreshAction = refreshAction;

            DeleteGroupCommand = new RelayCommand(() => DeleteGroup(SelectedGroup));
        }

        public GroupInfoViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                m_selectedGroup = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand DeleteGroupCommand { get; private set; }

        public bool InProgress
        {
            get { return m_inProgress; }
            set
            {
                m_inProgress = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowError
        {
            get { return m_showError; }
            set
            {
                m_showError = value;
                RaisePropertyChanged();
            }
        }

        private void DeleteGroup(GroupInfoViewModel selectedGroup)
        {
            ShowError = false;
            InProgress = true;
            m_dataService.RemoveGroup(selectedGroup.GroupId, exception =>
            {
                InProgress = false;
                if (exception != null)
                {
                    ShowError = true;
                    return;
                }

                m_refreshAction();
            });
        }
    }
}
