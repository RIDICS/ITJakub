using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.View.Groups;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationMenuPageViewModel : ViewModelBase
    {
        private bool m_isTeacherMode;
        public IDataService DataService { get; private set; }

        public INavigationService NavigationService { get; private set; }
        
        public ApplicationMenuPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            DataService = dataService;
            NavigationService = navigationService;

            LoadData();
            InitCommands();
        }
        
        private void LoadData()
        {
            DataService.GetLoggedUserInfo(false, loggedUser =>
            {
                IsTeacherMode = loggedUser.UserRole == UserRoleContract.Teacher;
            });
        }

        private void InitCommands()
        {
            OpenBookReaderCommand = new RelayCommand(Book.OpenLibrary);
            OpenMyGroupListCommand = new RelayCommand(() => NavigationService.Navigate<MyGroupListView>());
            OpenAdminGroupListCommand = new RelayCommand(() => NavigationService.Navigate<AdminGroupListView>());
            OpenMyTaskListCommand = new RelayCommand(() => NavigationService.Navigate<OwnedTaskListView>());
            CreateTaskCommand = new RelayCommand(() =>
            {
                DataService.SetAppSelectionTarget(SelectApplicationTarget.CreateTask);
                NavigationService.Navigate<SelectApplicationView>();
            });
        }

        public RelayCommand OpenBookReaderCommand { get; private set; }

        public RelayCommand OpenMyGroupListCommand { get; private set; }

        public RelayCommand OpenAdminGroupListCommand { get; private set; }

        public RelayCommand CreateTaskCommand { get; private set; }

        public RelayCommand OpenMyTaskListCommand { get; private set; }

        public bool IsTeacherMode
        {
            get { return m_isTeacherMode; }
            set
            {
                m_isTeacherMode = value;
                RaisePropertyChanged();
            }
        }
    }
}