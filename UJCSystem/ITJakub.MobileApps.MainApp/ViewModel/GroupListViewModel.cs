using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.MainApp.DataService;

namespace ITJakub.MobileApps.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GroupListViewModel : ViewModelBase
    {
        private IDataService m_dataService;
        private ObservableCollection<GroupInfo> m_groupList;
        private RelayCommand<ItemClickEventArgs> m_groupClickCommand;
        private RelayCommand m_createNewGroupCommand;
        private RelayCommand m_deleteGroupCommand;
        private RelayCommand m_refreshListCommand;
        private string m_connectToGroupNumber;
        private RelayCommand m_connectToGroupCommand;
        private string m_newGroupName;

        /// <summary>
        /// Initializes a new instance of the GroupListViewModel class.
        /// </summary>
        public GroupListViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            GroupList = new ObservableCollection<GroupInfo>();
            m_groupClickCommand = new RelayCommand<ItemClickEventArgs>(OpenGroup);
        }

        public ObservableCollection<GroupInfo> GroupList
        {
            get { return m_groupList; }
            set
            {
                m_groupList = value;
                RaisePropertyChanged(() => GroupList);
            }
        }

        public RelayCommand<ItemClickEventArgs> GroupClickCommand
        {
            get { return m_groupClickCommand; }
        }

        public RelayCommand CreateNewGroupCommand
        {
            get { return m_createNewGroupCommand; }
        }

        public RelayCommand DeleteGroupCommand
        {
            get { return m_deleteGroupCommand; }
        }

        public RelayCommand RefreshListCommand
        {
            get { return m_refreshListCommand; }
        }

        public RelayCommand Test
        {
            get { return new RelayCommand(() => GroupList.Add(new GroupInfo()));}
        }

        public string ConnectToGroupNumber
        {
            get { return m_connectToGroupNumber; }
            set
            {
                m_connectToGroupNumber = value;
                RaisePropertyChanged(() => ConnectToGroupNumber);
            }
        }

        public RelayCommand ConnectToGroupCommand
        {
            get { return m_connectToGroupCommand; }
        }

        public string NewGroupName
        {
            get { return m_newGroupName; }
            set
            {
                m_newGroupName = value;
                RaisePropertyChanged(() => NewGroupName);
            }
        }

        private void OpenGroup(ItemClickEventArgs args)
        {
            var group = args.ClickedItem as GroupInfo;
            if (group != null)
            {
                //TODO open group
                new MessageDialog("clicked").ShowAsync();
            }
        }
    }
}