using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.ComboBoxItem;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class OwnedTaskListViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private ObservableCollection<IGrouping<ApplicationType, TaskViewModel>> m_groupedTaskList;
        private ObservableCollection<TaskViewModel> m_taskList;
        private SortTaskType m_selectedSort;

        public OwnedTaskListViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            CreateNewTaskCommand = new RelayCommand(CreateNewTask);

            //TODO only for test:
            m_taskList = new ObservableCollection<TaskViewModel>
            {
                new TaskViewModel
                {
                    Application = ApplicationType.Hangman, Name = "Nazev 1", CreateTime = DateTime.Now
                },
                new TaskViewModel
                {
                    Application = ApplicationType.Fillwords, Name = "Nazev 2", CreateTime = DateTime.Now.AddMinutes(-29)
                },
                new TaskViewModel
                {
                    Application = ApplicationType.Fillwords, Name = "Nazev 3", CreateTime = DateTime.Now
                },
                new TaskViewModel
                {
                    Application = ApplicationType.Crosswords, Name = "Nazev 4", CreateTime = DateTime.Now
                }
            };

            GroupAndSortTaskList(m_taskList);
        }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand CreateNewTaskCommand { get; private set; }

        public ObservableCollection<IGrouping<ApplicationType, TaskViewModel>> GroupedTaskList
        {
            get { return m_groupedTaskList; }
            set
            {
                m_groupedTaskList = value;
                RaisePropertyChanged();
            }
        }

        public SortTaskType SelectedSort
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

        private void GroupAndSortTaskList(IEnumerable<TaskViewModel> taskList)
        {
            if (taskList == null)
                return;

            IOrderedEnumerable<TaskViewModel> sortedTaskList;
            switch (SelectedSort)
            {
                case SortTaskType.Name:
                    sortedTaskList = taskList.OrderBy(model => model.Name);
                    break;
                case SortTaskType.CreateTime:
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
            m_dataService.SetAppSelectionTarget(ApplicationSelectionTarget.CreateTask);
            m_navigationService.Navigate<ApplicationSelectionView>();
        }

        //private void OpenEditor(ApplicationType applicationType) //TODO remov
        //{
        //    m_navigationService.Navigate(typeof(EditorHostView));
        //    MessengerInstance.Send(new OpenEditorMessage {Application = applicationType});
        //}
    }
}
