using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Application;
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
        private readonly IErrorService m_errorService;
        private ApplicationType m_applicationType;
        private bool m_noTaskExists;
        private bool m_loading;
        private string m_applicationName;
        private bool m_saving;
        private ObservableCollection<TaskViewModel> m_originalTaskList;
        private ObservableCollection<IGrouping<bool, TaskViewModel>> m_groupedTaskList;
        private string m_searchQuery;
        private bool m_noSearchResults;
        private bool m_isSearchingMode;

        public SelectTaskViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            m_dataService.SetAppSelectionTarget(SelectApplicationTarget.SelectTask);

            InitCommands();
            LoadData();
        }
        
        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() => m_navigationService.GoBack());
            TaskClickCommand = new RelayCommand<ItemClickEventArgs>(TaskClick);
            CreateNewTaskCommand = new RelayCommand(CreateNewTask);
            RefreshListCommand = new RelayCommand(LoadTasks);
            SearchCommand = new RelayCommand(Search);
            CancelSearchCommand = new RelayCommand(CancelSearch);
        }
        
        private void LoadData()
        {
            NoTaskExists = false;
            m_dataService.GetCurrentApplication(type =>
            {
                m_applicationType = type;
                m_dataService.GetApplication(type, (app, exception) =>
                {
                    if (exception != null)
                    {
                        ApplicationName = "(Neznámá aplikace)";
                        return;
                    }

                    ApplicationName = app.Name;
                });

                LoadTasks();
            });
        }

        private void LoadTasks()
        {
            Loading = true;
            m_dataService.GetTasksByApplication(m_applicationType, (taskList, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                m_originalTaskList = taskList;
                NoTaskExists = taskList.Count == 0;
                SetGroupedTaskList(m_originalTaskList);
            });
        }
        
        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand<ItemClickEventArgs> TaskClickCommand { get; private set; }

        public RelayCommand CreateNewTaskCommand { get; private set; }
        
        public RelayCommand RefreshListCommand { get; private set; }

        public RelayCommand SearchCommand { get; private set; }

        public RelayCommand CancelSearchCommand { get; private set; }

        public ObservableCollection<IGrouping<bool, TaskViewModel>> GroupedTaskList
        {
            get { return m_groupedTaskList; }
            set
            {
                m_groupedTaskList = value;
                RaisePropertyChanged();
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

        public string SearchQuery
        {
            get { return m_searchQuery; }
            set
            {
                m_searchQuery = value;
                RaisePropertyChanged();
            }
        }

        public bool NoSearchResults
        {
            get { return m_noSearchResults; }
            set
            {
                m_noSearchResults = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSearchingMode
        {
            get { return m_isSearchingMode; }
            set
            {
                m_isSearchingMode = value;
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
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                m_dataService.SetRestoringLastGroupState(false);
                
                m_navigationService.GoBack();
                m_navigationService.GoBack(); // go to page before ApplicationSelection
            });
        }

        private void CreateNewTask()
        {
            m_navigationService.Navigate<EditorHostView>();
        }
        
        private void Search()
        {
            IsSearchingMode = false;
            if (m_originalTaskList == null || m_originalTaskList.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                SetGroupedTaskList(m_originalTaskList);
                return;
            }

            IsSearchingMode = true;
            SetGroupedTaskList(m_originalTaskList.Where(task => task.Name.ToLower().Contains(SearchQuery.ToLower())));
        }

        private void CancelSearch()
        {
            IsSearchingMode = false;
            SetGroupedTaskList(m_originalTaskList);
            SearchQuery = string.Empty;
        }

        private void SetGroupedTaskList(IEnumerable<TaskViewModel> tasks)
        {
            GroupedTaskList =
                new ObservableCollection<IGrouping<bool, TaskViewModel>>(
                    tasks.GroupBy(task => task.Author.IsMe).OrderByDescending(taskGroup => taskGroup.Key));
        }
    }
}