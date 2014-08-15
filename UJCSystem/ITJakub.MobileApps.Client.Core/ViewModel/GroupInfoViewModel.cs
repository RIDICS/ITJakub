using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class GroupInfoViewModel : ViewModelBase
    {
        private ObservableCollection<GroupMemberViewModel> m_members;
        public string GroupName { get; set; }
        public int MemberCount { get; set; }
        public string GroupCode { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public BitmapImage Icon { get; set; }
        public string ApplicationName { get; set; }
        public long GroupId { get; set; }
        public GroupType GroupType { get; set; }
        public DateTime CreateTime { get; set; }

        public ObservableCollection<GroupMemberViewModel> Members
        {
            get { return m_members; }
            set
            {
                m_members = value;
                RaisePropertyChanged();
            }
        }
    }
}