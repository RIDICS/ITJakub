using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.DataService;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu
{
    public class UserMenuViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_lastName;
        private string m_firstName;
        private ImageSource m_userAvatar;
        private readonly NavigationService m_navigationService;

        public UserMenuViewModel(IDataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            InitializeCommands();
            LoadInitData();
        }

        public RelayCommand LogOutCommand { get; private set; }

        private void LoadInitData()
        {
            m_dataService.GetLoggedUserInfo((userInfo, exception) =>
            {
                if (exception != null)
                    return;
                

                FirstName = userInfo.FirstName;
                LastName = userInfo.LastName;
                UserAvatar = userInfo.UserAvatar;
            });
        }

        private void InitializeCommands()
        {
            LogOutCommand = new RelayCommand(() =>
            {
                m_dataService.LogOut();
                m_navigationService.GoHome();
                Messenger.Default.Send(new LogOutMessage());
            });
        }


        public string FirstName
        {
            get { return m_firstName; }
            set
            {
                m_firstName = value;
                RaisePropertyChanged();
            }
        }

        public string LastName
        {
            get { return m_lastName; }
            set
            {
                m_lastName = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource UserAvatar
        {
            get { return m_userAvatar; }
            set { m_userAvatar = value; RaisePropertyChanged();}
        }
    }
}