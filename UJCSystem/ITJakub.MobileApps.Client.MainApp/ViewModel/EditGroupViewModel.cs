using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class EditGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private ObservableCollection<AppInfoViewModel> m_appList;
        private AppInfoViewModel m_selectedGroup;

        /// <summary>
        /// Initializes a new instance of the EditGroupViewModel class.
        /// </summary>
        public EditGroupViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            AppList = new ObservableCollection<AppInfoViewModel>();
            LoadAppList();
        }

        private void LoadAppList()
        {
            m_dataService.GetAllApplications((applications, exception) =>
            {
                if (exception != null)
                    return;
                AppList.Clear();
                foreach (var applicationKeyValue in applications.Where(pair => pair.Value.ApplicationRoleType == ApplicationRoleType.MainApp))
                {
                    AppList.Add(new AppInfoViewModel
                    {
                        Name = applicationKeyValue.Value.Name,
                        ApplicationType = applicationKeyValue.Key,
                        Icon = applicationKeyValue.Value.Icon
                    });
                }
            });
        }

        public ObservableCollection<AppInfoViewModel> AppList
        {
            get { return m_appList; }
            set
            {
                m_appList = value;
                RaisePropertyChanged();
            }
        }

        public AppInfoViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                m_selectedGroup = value;
                RaisePropertyChanged();
            }
        }
    }
}