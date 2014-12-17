using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
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
        private string m_applicationName;
        private bool m_saving;

        public SelectTaskViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            
            NoTaskExists = false;
            InitCommands();

            m_dataService.GetCurrentApplication(type =>
            {
                m_dataService.GetApplication(type, (app, exception) =>
                {
                    if (exception != null)
                        return;

                    ApplicationName = app.Name;
                });

                LoadTasks(type);
            });
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() => m_navigationService.GoBack());
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

        public RelayCommand CreateNewTaskCommand { get; private set; }

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

        public string ApplicationName
        {
            get { return m_applicationName; }
            set
            {
                m_applicationName = value;
                RaisePropertyChanged();
            }
        }

        public bool Saving
        {
            get { return m_saving; }
            set
            {
                m_saving = value;
                RaisePropertyChanged();
            }
        }

        private void TaskClick(ItemClickEventArgs args)
        {
            var task = args.ClickedItem as TaskViewModel;
            if (task != null)
            {
                SaveTask(task);
            }
        }

        private void SaveTask(TaskViewModel task)
        {
            Saving = true;
            m_dataService.AssignTaskToCurrentGroup(task.Id, exception =>
            {
                Saving = false;
                if (exception != null)
                    return;

                m_dataService.SetRestoringLastGroupState(false);
                
                m_navigationService.GoBack();
                m_navigationService.GoBack(); // go to page before ApplicationSelection
            });
        }

        private void CreateNewTask()
        {
            m_navigationService.Navigate(typeof(EditorHostView));
        }
    }
}