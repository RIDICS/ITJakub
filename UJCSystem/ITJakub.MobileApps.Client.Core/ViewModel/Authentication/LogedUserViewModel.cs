using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Core.ViewModel.Authentication
{
    public class LogedUserViewModel:ViewModelBase
    {
        private string m_firstName;
        private string m_lastName;
        private ImageSource m_userAvatar;

        public string FirstName
        {
            get { return m_firstName; }
            set { m_firstName = value; RaisePropertyChanged();}
        }

        public string LastName
        {
            get { return m_lastName; }
            set { m_lastName = value; RaisePropertyChanged();}
        }

        public ImageSource UserAvatar
        {
            get { return m_userAvatar; }
            set { m_userAvatar = value; RaisePropertyChanged();}
        }
    }
}
