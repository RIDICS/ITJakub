using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.ComboBoxItem;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class OwnedTaskListViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private ObservableCollection<IGrouping<ApplicationType, TaskViewModel>> m_groupedTaskList;
        private ObservableCollection<TaskViewModel> m_taskList;
        private SortTaskItem.SortType m_selectedSort;
        private bool m_loading;
        private bool m_noTask;

        public OwnedTaskListViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            CreateNewTaskCommand = new RelayCommand(CreateNewTask);
            RefreshListCommand = new RelayCommand(LoadTasks);
            TaskClickCommand = new RelayCommand<ItemClickEventArgs>(OpenTaskPreview);

            LoadTasks();
        }
        
        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand CreateNewTaskCommand { get; private set; }

        public RelayCommand RefreshListCommand { get; private set; }

        public RelayCommand<ItemClickEventArgs> TaskClickCommand { get; private set; }

        public ObservableCollection<IGrouping<ApplicationType, TaskViewModel>> GroupedTaskList
        {
            get { return m_groupedTaskList; }
            set
            {
                m_groupedTaskList = value;
                RaisePropertyChanged();
            }
        }

        public SortTaskItem.SortType SelectedSort
        {
            get { return m_selectedSort; }
            set
            {
                m_selectedSort = value;
                GroupAndSortTaskList(m_taskList);
                RaisePropertyChanged();
            }
        }

        public int DefaultIndex
        {
            get { return 0; }
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

        public bool NoTask
        {
            get { return m_noTask; }
            set
            {
                m_noTask = value;
                RaisePropertyChanged();
            }
        }
        
        private void LoadTasks()
        {
            Loading = true;
            m_dataService.GetMyTasks((taskList, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                m_taskList = taskList;
                NoTask = taskList.Count == 0;
                GroupAndSortTaskList(m_taskList);
            });
        }

        private void GroupAndSortTaskList(IEnumerable<TaskViewModel> taskList)
        {
            if (taskList == null)
                return;

            IOrderedEnumerable<TaskViewModel> sortedTaskList;
            switch (SelectedSort)
            {
                case SortTaskItem.SortType.Name:
                    sortedTaskList = taskList.OrderBy(model => model.Name);
                    break;
                case SortTaskItem.SortType.CreateTime:
                    sortedTaskList = taskList.OrderByDescending(model => model.CreateTime);
                    break;
                default:
                    sortedTaskList = taskList.OrderBy(model => model.Name);
                    break;
            }

            GroupedTaskList = new ObservableCollection<IGrouping<ApplicationType, TaskViewModel>>(sortedTaskList.GroupBy(model => model.Application).OrderBy(models => models.Key, new ApplicationTypeComparator()));
        }

        private void CreateNewTask()
        {
            m_dataService.SetAppSelectionTarget(SelectApplicationTarget.CreateTask);
            m_navigationService.Navigate<SelectApplicationView>();
        }

        private void OpenTaskPreview(ItemClickEventArgs args)
        {
            var item = args.ClickedItem as TaskViewModel;
            if (item == null)
                return;
            
            m_navigationService.OpenPopup<TaskPreviewHostView>();
            Messenger.Default.Send(new SelectedTaskMessage {TaskViewModel = item});
        }
    }
}
