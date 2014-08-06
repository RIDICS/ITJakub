using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.DataService;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GroupPageViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private string m_groupName;

        /// <summary>
        /// Initializes a new instance of the GroupPageViewModel class.
        /// </summary>
        public GroupPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
        }

        public string GroupName
        {
            get { return m_groupName; }
            set
            {
                m_groupName = value;
                RaisePropertyChanged();
            }
        }
    }
}