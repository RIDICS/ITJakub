using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GroupPageViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private AppInfoViewModel m_selectedApplicationInfo;
        private GroupInfoViewModel m_groupInfo;
        private TaskViewModel m_selectedTaskViewModel;
        private bool m_saving;
        private bool m_taskSaved;
        private bool m_taskNotSelectedError;

        /// <summary>
        /// Initializes a new instance of the GroupPageViewModel class.
        /// </summary>
        public GroupPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                LoadData(message.Group);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });

            InitCommands();
            //TODO polling group members
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() =>
            {
                Messenger.Default.Unregister(this);
                m_navigationService.GoBack();
            });
            SelectAppCommand = new RelayCommand(SelectApplication);
            SelectTaskCommand = new RelayCommand(SelectTask);
            SaveTaskCommand = new RelayCommand(SaveTask);
        }

        private void LoadData(GroupInfoViewModel group)
        {
            GroupInfo = group;
            //TODO load current task
        }

        public GroupInfoViewModel GroupInfo
        {
            get { return m_groupInfo; }
            set
            {
                m_groupInfo = value;
                RaisePropertyChanged();
            }
        }

        public AppInfoViewModel SelectedApplicationInfo
        {
            get { return m_selectedApplicationInfo; }
            set
            {
                m_selectedApplicationInfo = value;
                RaisePropertyChanged();
            }
        }

        public TaskViewModel SelectedTaskViewModel
        {
            get { return m_selectedTaskViewModel; }
            set
            {
                m_selectedTaskViewModel = value;
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

        public bool TaskSaved
        {
            get { return m_taskSaved; }
            set
            {
                m_taskSaved = value;
                RaisePropertyChanged();
            }
        }

        public bool TaskNotSelectedError
        {
            get { return m_taskNotSelectedError; }
            set
            {
                m_taskNotSelectedError = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand SelectAppCommand { get; private set; }

        public RelayCommand SelectTaskCommand { get; private set; }

        public RelayCommand SaveTaskCommand { get; private set; }

        private void SelectApplication()
        {
            SelectedTaskViewModel = null;
            HideInfoAndErrors();
            
            m_navigationService.Navigate(typeof(ApplicationSelectionView));
            Messenger.Default.Register<SelectedApplicationMessage>(this, message =>
            {
                SelectedApplicationInfo = message.AppInfo;
                Messenger.Default.Unregister<SelectedApplicationMessage>(this);
            });
        }

        private void SelectTask()
        {
            HideInfoAndErrors();
            if (SelectedApplicationInfo == null)
            {
                SelectAppAndTask();
                return;
            }

            m_navigationService.Navigate(typeof(SelectTaskView));
            Messenger.Default.Send(new SelectedApplicationMessage
            {
                AppInfo = SelectedApplicationInfo
            });
            Messenger.Default.Register<SelectedTaskMessage>(this, message =>
            {
                SelectedTaskViewModel = message.TaskInfo;
                Messenger.Default.Unregister<SelectTaskView>(this);
            });
        }

        private void SelectAppAndTask()
        {
            m_navigationService.Navigate(typeof(ApplicationSelectionView));
            Messenger.Default.Register<SelectedApplicationMessage>(this, message =>
            {
                SelectedApplicationInfo = message.AppInfo;
                Messenger.Default.Unregister<SelectedApplicationMessage>(this);
                SelectTask();
            });
        }

        private void HideInfoAndErrors()
        {
            TaskSaved = false;
            TaskNotSelectedError = false;
            Saving = false;
        }

        private void SaveTask()
        {
            HideInfoAndErrors();

            if (SelectedTaskViewModel == null)
            {
                TaskNotSelectedError = true;
                return;
            }

            Saving = true;
            m_dataService.AssignTaskToGroup(m_groupInfo.GroupId, m_selectedTaskViewModel.Id, exception =>
            {
                Saving = false;
                if (exception != null)
                    return;

                TaskSaved = true;
            });
        }
    }
}