using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class AdminGroupListViewModel : GroupListViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private readonly IMainPollingService m_pollingService;
        private List<GroupInfoViewModel> m_selectedGroups;
        private bool m_isOneItemSelected;
        private bool m_canCloseSelected;
        private bool m_canStartSelected;
        private bool m_canRemoveSelected;
        private bool m_canPauseSelected;
        private bool m_isAtLeastOneSelected;
        
        public AdminGroupListViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;
            m_pollingService = pollingService;

            m_selectedGroups = new List<GroupInfoViewModel>();
            
            InitCommands();
            InitViewModels();
            LoadData();
        }
        
        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() =>
            {
                m_pollingService.UnregisterAll();
                m_navigationService.GoBack();
            });
            SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(SelectionChanged);
            OpenMyTaskListCommand = new RelayCommand(Navigate<OwnedTaskListView>);
            CreateTaskCommand = new RelayCommand(CreateNewTask);
            RefreshListCommand = new RelayCommand(LoadData);
        }

        private void InitViewModels()
        {
            CreateNewGroupViewModel = new CreateGroupViewModel(m_dataService, m_errorService, Navigate<GroupPageView>);
            SwitchToRunningViewModel = new SwitchGroupStateViewModel(GroupStateContract.Running, m_dataService, m_selectedGroups, LoadData, m_errorService);
            SwitchToPauseViewModel = new SwitchGroupStateViewModel(GroupStateContract.Paused, m_dataService, m_selectedGroups, LoadData, m_errorService);
            SwitchToClosedViewModel = new SwitchGroupStateViewModel(GroupStateContract.Closed, m_dataService, m_selectedGroups, LoadData, m_errorService);
            DeleteGroupViewModel = new DeleteGroupViewModel(m_dataService, m_selectedGroups, LoadData, m_errorService);
            DuplicateGroupViewModel = new DuplicateGroupViewModel(m_dataService, m_selectedGroups);
        }

        private void LoadData()
        {
            m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);

            Loading = true;
            m_dataService.GetOwnedGroupsForCurrentUser((groupList, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionWarning();
                    return;
                }

                m_completeGroupList = groupList;
                DisplayGroupList();

                m_pollingService.RegisterForGroupsUpdate(UpdatePollingInterval, m_completeGroupList, GroupUpdate);
            });
        }

        private void Navigate<T>()
        {
            m_pollingService.UnregisterAll();
            m_navigationService.Navigate<T>();
        }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }
        
        public RelayCommand OpenMyTaskListCommand { get; private set; }

        public RelayCommand CreateTaskCommand { get; private set; }

        public RelayCommand RefreshListCommand { get; private set; }


        public CreateGroupViewModel CreateNewGroupViewModel { get; set; }

        public SwitchGroupStateViewModel SwitchToPauseViewModel { get; set; }

        public SwitchGroupStateViewModel SwitchToRunningViewModel { get; set; }

        public SwitchGroupStateViewModel SwitchToClosedViewModel { get; set; }

        public DeleteGroupViewModel DeleteGroupViewModel { get; set; }

        public DuplicateGroupViewModel DuplicateGroupViewModel { get; set; }

        public bool IsOneItemSelected
        {
            get { return m_isOneItemSelected; }
            set
            {
                m_isOneItemSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool CanCloseSelected
        {
            get { return m_canCloseSelected; }
            set { m_canCloseSelected = value; RaisePropertyChanged(); }
        }

        public bool CanStartSelected
        {
            get { return m_canStartSelected; }
            set { m_canStartSelected = value; RaisePropertyChanged(); }
        }

        public bool CanRemoveSelected
        {
            get { return m_canRemoveSelected; }
            set { m_canRemoveSelected = value && m_isOneItemSelected; RaisePropertyChanged(); }
        }

        public bool CanPauseSelected
        {
            get { return m_canPauseSelected; }
            set { m_canPauseSelected = value; RaisePropertyChanged(); }
        }

        public bool IsAtLeastOneSelected
        {
            get { return m_isAtLeastOneSelected; }
            set { m_isAtLeastOneSelected = value; RaisePropertyChanged(); }
        }


        protected override void OpenGroup(GroupInfoViewModel group)
        {
            if (group == null)
                return;

            m_dataService.SetCurrentGroup(group.GroupId, GroupType.Owner);
            Navigate<GroupPageView>();
        }

        public override bool IsTeacherView { get { return true; } }
        
        private void SelectionChanged(SelectionChangedEventArgs args)
        {
            foreach (var removedItem in args.RemovedItems)
            {
                m_selectedGroups.Remove(removedItem as GroupInfoViewModel);
            }
            foreach (var addedItem in args.AddedItems)
            {
                m_selectedGroups.Add(addedItem as GroupInfoViewModel);
            }
            IsAtLeastOneSelected = m_selectedGroups.Count > 0;
            IsOneItemSelected = m_selectedGroups.Count == 1;
            DeleteGroupViewModel.SelectedGroupCount = m_selectedGroups.Count;

            UpdateGroupStateActions();
        }

        private void UpdateGroupStateActions()
        {
            var stateCount = m_selectedGroups.GroupBy(model => model.State).ToDictionary(models => models.Key, models => models.Count());
            var totalCount = m_selectedGroups.Count();

            for (var state = GroupStateContract.Created; state <= GroupStateContract.Closed; state++)
            {
                if (!stateCount.ContainsKey(state))
                    stateCount[state] = 0;
            }
            CanPauseSelected = stateCount[GroupStateContract.Running] == totalCount;
            CanRemoveSelected = stateCount[GroupStateContract.Created] + stateCount[GroupStateContract.Closed] == totalCount;
            CanStartSelected = stateCount[GroupStateContract.WaitingForStart] + stateCount[GroupStateContract.Paused] == totalCount;
            CanCloseSelected = stateCount[GroupStateContract.Running] + stateCount[GroupStateContract.Paused] == totalCount;
        }
        
        private void CreateNewTask()
        {
            m_dataService.SetAppSelectionTarget(SelectApplicationTarget.CreateTask);
            Navigate<SelectApplicationView>();
        }

        private void GroupUpdate(Exception exception)
        {
            if (exception != null)
                m_errorService.ShowConnectionWarning();
            else
                m_errorService.HideWarning();
        }
    }
}
