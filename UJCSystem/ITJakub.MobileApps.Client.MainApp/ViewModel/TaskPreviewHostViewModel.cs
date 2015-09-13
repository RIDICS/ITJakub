using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class TaskPreviewHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly IErrorService m_errorService;
        private readonly NavigationService m_navigationService;
        private string m_appName;
        private string m_taskName;
        private TaskPreviewBaseViewModel m_taskPreviewHostViewModel;
        private bool m_loading;

        public TaskPreviewHostViewModel(IDataService dataService, IErrorService errorService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_errorService = errorService;
            m_navigationService = navigationService;

            Messenger.Default.Register<SelectedTaskMessage>(this, LoadData);
            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() => m_navigationService.ClosePopup());
        }

        private void LoadData(SelectedTaskMessage message)
        {
            var task = message.TaskViewModel;
            m_dataService.GetApplication(task.Application, (appInfo, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                AppName = appInfo.Name;
                TaskName = task.Name;
                TaskPreviewViewModel = appInfo.TaskPreviewViewModel;

                LoadTask(task.Id);
            });
        }

        private void LoadTask(long taskId)
        {
            Loading = true;
            m_dataService.GetTask(taskId, (task, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionWarning();
                    return;
                }

                TaskPreviewViewModel.ShowTask(task.Data);
            });
        }

        public RelayCommand GoBackCommand { get; private set; }

        public string AppName
        {
            get { return m_appName; }
            set
            {
                m_appName = value;
                RaisePropertyChanged();
            }
        }

        public string TaskName
        {
            get { return m_taskName; }
            set
            {
                m_taskName = value;
                RaisePropertyChanged();
            }
        }

        public TaskPreviewBaseViewModel TaskPreviewViewModel
        {
            get { return m_taskPreviewHostViewModel; }
            set
            {
                m_taskPreviewHostViewModel = value;
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
    }
}
