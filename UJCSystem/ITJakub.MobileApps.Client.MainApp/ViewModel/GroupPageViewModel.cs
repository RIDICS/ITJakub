﻿using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

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
        private const PollingInterval MembersPollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;
        private AppInfoViewModel m_selectedApplicationInfo;
        private GroupInfoViewModel m_groupInfo;
        private TaskViewModel m_selectedTaskViewModel;
        private bool m_saving;
        private bool m_taskSaved;
        private bool m_loading;

        /// <summary>
        /// Initializes a new instance of the GroupPageViewModel class.
        /// </summary>
        public GroupPageViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;
            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                LoadData(message.Group);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });

            SelectedApplicationInfo = new AppInfoViewModel();
            SelectedTaskViewModel = new TaskViewModel();

            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() =>
            {
                m_pollingService.Unregister(MembersPollingInterval, UpdateMembers);
                Messenger.Default.Unregister(this);
                m_navigationService.GoBack();
            });

            SelectAppAndTaskCommand = new RelayCommand(SelectAppAndTask);
        }

        private void LoadData(GroupInfoViewModel group)
        {
            GroupInfo = group;

            Loading = true;
            m_dataService.OpenGroupAndGetDetails(group.GroupId, (groupInfo, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                GroupInfo = groupInfo;
                if (groupInfo.Task == null)
                {
                    SelectedTaskViewModel = new TaskViewModel();
                    SelectedApplicationInfo = new AppInfoViewModel {ApplicationType = ApplicationType.Unknown};
                }
                else
                {
                    SelectedTaskViewModel = groupInfo.Task;
                    SelectedApplicationInfo = new AppInfoViewModel {ApplicationType = groupInfo.Task.Application};
                }
                
                m_pollingService.RegisterForGroupsUpdate(MembersPollingInterval, new[] {GroupInfo}, UpdateMembers);
            });
        }

        private void UpdateMembers(Exception exception)
        {
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

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand SelectAppAndTaskCommand { get; private set; }

        private void SelectAppAndTask()
        {
            Saving = false;
            TaskSaved = false;

            m_navigationService.Navigate(typeof(ApplicationSelectionView));
            Messenger.Default.Register<SelectedApplicationMessage>(this, message =>
            {
                Messenger.Default.Unregister<SelectedApplicationMessage>(this);
                SelectedApplicationInfo = message.AppInfo;
                SelectTask(message.AppInfo);
            });
        }

        private void SelectTask(AppInfoViewModel application)
        {
            m_navigationService.Navigate(typeof(SelectTaskView));

            Messenger.Default.Send(new SelectedApplicationMessage {AppInfo = application});

            Messenger.Default.Register<SelectedTaskMessage>(this, message =>
            {
                Messenger.Default.Unregister<SelectTaskView>(this);
                SelectedTaskViewModel = message.TaskInfo;
                SaveTask(message.TaskInfo);
            });
        }

        private void SaveTask(TaskViewModel task)
        {
            Saving = true;
            m_dataService.AssignTaskToGroup(m_groupInfo.GroupId, task.Id, exception =>
            {
                Saving = false;
                if (exception != null)
                    return;

                TaskSaved = true;
            });
        }
    }
}