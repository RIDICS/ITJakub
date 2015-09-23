using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class DuplicateGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly List<GroupInfoViewModel> m_selectedGroups;
        private bool m_showError;
        private bool m_inProgress;

        public DuplicateGroupViewModel(IDataService dataService, List<GroupInfoViewModel> selectedGroups)
        {
            m_dataService = dataService;
            m_selectedGroups = selectedGroups;
            SubmitCommand = new RelayCommand(DuplicateGroup);
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

        public bool InProgress
        {
            get { return m_inProgress; }
            set
            {
                m_inProgress = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SubmitCommand { get; private set; }

        private void DuplicateGroup()
        {
            
        }
    }
}