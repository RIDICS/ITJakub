using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class GroupInfoViewModel
    {
        public string GroupName { get; set; }
        public int MemberCount { get; set; }
        public string GroupCode { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public BitmapImage Icon { get; set; }
        public string ApplicationName { get; set; }
        public long GroupId { get; set; }
        public GroupType GroupType { get; set; }
    }
}