using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class GroupMemberViewModel : ViewModelBase
    {
        private ImageSource m_userAvatar;

        public ImageSource UserAvatar
        {
            get { return m_userAvatar; }
            set
            {
                m_userAvatar = value;
                RaisePropertyChanged();
            }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long Id { get; set; }
    }
}