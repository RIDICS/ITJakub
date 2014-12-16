using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class SelectTaskViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private ObservableCollection<TaskViewModel> m_taskList;
        private bool m_noTaskExists;
        private bool m_loading;

        public SelectTaskViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            
            NoTaskExists = false;
            InitCommands();

            Messenger.Default.Register<SelectedApplicationMessage>(this, message =>
            {
                Messenger.Default.Unregister(this);
                SelectedApplication = message.AppInfo;
                LoadTasks(message.AppInfo.ApplicationType);
            });
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() => m_navigationService.GoBackUsingCache());
            TaskClickCommand = new RelayCommand<ItemClickEventArgs>(TaskClick);
            CreateNewTaskCommand = new RelayCommand(CreateNewTask);
        }
        
        private void LoadTasks(ApplicationType applicationType)
        {
            Loading = true;
            m_dataService.GetTasksByApplication(applicationType, (taskList, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                TaskList = taskList;
            });
        }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand<ItemClickEventArgs> TaskClickCommand { get; private set; }

        public AppInfoViewModel SelectedApplication { get; set; }

        public ObservableCollection<TaskViewModel> TaskList
        {
            get { return m_taskList; }
            set
            {
                m_taskList = value;
                RaisePropertyChanged();
                NoTaskExists = m_taskList.Count == 0;
            }
        }

        public bool NoTaskExists
        {
            get { return m_noTaskExists; }
            set
            {
                m_noTaskExists = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CreateNewTaskCommand { get; private set; }

        private void TaskClick(ItemClickEventArgs args)
        {
            var task = args.ClickedItem as TaskViewModel;
            if (task != null)
            {
                m_navigationService.GoBackUsingCache();
                Messenger.Default.Send(new SelectedTaskMessage {TaskInfo = task});
            }
        }

        private void CreateNewTask()
        {
            m_navigationService.Navigate(typeof(EditorHostView));
            MessengerInstance.Send(new OpenEditorMessage { Application = SelectedApplication.ApplicationType });
        }
    }
}