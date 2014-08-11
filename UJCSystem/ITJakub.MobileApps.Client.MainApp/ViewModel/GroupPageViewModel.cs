using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;

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
        private string m_groupCode;
        private ObservableCollection<GroupMemberViewModel> m_memberList;

        /// <summary>
        /// Initializes a new instance of the GroupPageViewModel class.
        /// </summary>
        public GroupPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                LoadData(message.Group.GroupId);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });
        }

        private void LoadData(long groupId)
        {
            m_dataService.GetGroupDetails(groupId, (group, exception) =>
            {
                if (exception != null)
                    return;

                GroupName = group.GroupName;
                MemberList = group.Members;
            });
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

        public string GroupCode
        {
            get { return m_groupCode; }
            set { m_groupCode = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<GroupMemberViewModel> MemberList
        {
            get { return m_memberList; }
            set { m_memberList = value; RaisePropertyChanged(); }
        }
    }
}