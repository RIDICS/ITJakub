using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu
{
    public class UserMenuViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_lastName;
        private string m_firstName;

        public UserMenuViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            InitializeCommands();
            LoadInitData();
        }

        public RelayCommand LogOutCommand { get; private set; }

        private void LoadInitData()
        {
            m_dataService.GetUserInfo((userInfo, exception) =>
            {
                if (exception != null)
                    return;
                FirstName = userInfo.FirstName;
                LastName = userInfo.LastName;
            });
        }

        private void InitializeCommands()
        {
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
    }
}