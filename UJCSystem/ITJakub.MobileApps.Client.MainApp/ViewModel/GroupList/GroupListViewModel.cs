﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.ComboBoxItem;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class GroupListViewModel : ViewModelBase
    {
        private const PollingInterval UpdatePollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;

        private readonly List<GroupInfoViewModel> m_selectedGroups;
        private GroupInfoViewModel m_selectedGroup;
        private ObservableCollection<GroupInfoViewModel> m_groups;
        private ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>> m_groupList;
        private SortGroupItem.SortType m_selectedSortType;
        private GroupStateContract? m_currentFilter;
        private UserRoleContract m_userRole;
        private bool m_isCommandBarOpen;
        private bool m_loading;
        private bool m_isOneItemSelected;
        private bool m_onlyOwnedGroupsSelected;
        private bool m_isFilter;
        
        public GroupListViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;

            m_selectedGroups = new List<GroupInfoViewModel>();
            GroupList = new ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>>();
            m_userRole = UserRoleContract.Student;
            
            Messenger.Default.Register<LogOutMessage>(this, message =>
            {
                m_pollingService.UnregisterAll();
                Messenger.Default.Unregister(this);
            });

            InitViewModels();
            InitCommands();
            LoadData();
        }

        #region Properties

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; private set; }

        public RelayCommand<ItemClickEventArgs> GroupClickCommand { get; private set; }

        public RelayCommand RefreshListCommand { get; private set; }

        public RelayCommand ConnectCommand { get; private set; }

        public RelayCommand OpenTaskEditorCommand { get; private set; }

        public RelayCommand<object> FilterCommand { get; private set; }

        public ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>> GroupList
        {
            get { return m_groupList; }
            set
            {
                m_groupList = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => IsGroupListEmpty);
            }
        }

        public bool IsCommandBarOpen
        {
            get { return m_isCommandBarOpen; }
            set
            {
                m_isCommandBarOpen = value;
                RaisePropertyChanged();
            }
        }

        public GroupInfoViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                m_selectedGroup = value;
                IsCommandBarOpen = value != null;
                DeleteGroupViewModel.SelectedGroup = value;

                RaisePropertyChanged();
                RaisePropertyChanged(() => IsGroupSelected);
                RaisePropertyChanged(() => IsTeacherAndGroupSelected);
            }
        }

        public bool IsGroupSelected
        {
            get { return SelectedGroup != null; }
        }

        public bool IsTeacherAndGroupSelected
        {
            get { return m_userRole == UserRoleContract.Teacher && SelectedGroup != null; }
        }

        public bool IsTeacherMode
        {
            get { return m_userRole == UserRoleContract.Teacher; }
        }

        public bool IsGroupListEmpty
        {
            get { return m_groupList.Count == 0; }
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

        public bool IsOneItemSelected
        {
            get { return m_isOneItemSelected; }
            set
            {
                m_isOneItemSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool OnlyOwnedGroupsSelected
        {
            get { return m_onlyOwnedGroupsSelected; }
            set
            {
                m_onlyOwnedGroupsSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFilter
        {
            get { return m_isFilter; }
            set
            {
                m_isFilter = value;
                RaisePropertyChanged();
            }
        }

        public GroupStateContract? CurrentFilter
        {
            get { return m_currentFilter; }
            set
            {
                m_currentFilter = value;
                RaisePropertyChanged();
            }
        }

        public SortGroupItem.SortType SelectedSortType
        {
            get { return m_selectedSortType; }
            set
            {
                m_selectedSortType = value;
                RaisePropertyChanged();

                if (m_groups != null)
                    DisplayGroupList(m_groups);
            }
        }

        public int DefaultIndex
        {
            get { return 0; }
        }

        public ConnectToGroupViewModel ConnectToGroupViewModel { get; set; }

        public CreateGroupViewModel CreateNewGroupViewModel { get; set; }

        public DeleteGroupViewModel DeleteGroupViewModel { get; set; }

        public SwitchGroupStateViewModel SwitchToPauseViewModel { get; set; }

        public SwitchGroupStateViewModel SwitchToRunningViewModel { get; set; }

        #endregion

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack);
            GroupClickCommand = new RelayCommand<ItemClickEventArgs>(GroupClick);
            SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(SelectionChanged);
            ConnectCommand = new RelayCommand(() => OpenGroup(SelectedGroup));
            RefreshListCommand = new RelayCommand(LoadData);
            OpenTaskEditorCommand = new RelayCommand(() => Navigate(typeof(OwnedTaskListView)));
            FilterCommand = new RelayCommand<object>(Filter);
        }

        private void InitViewModels()
        {
            ConnectToGroupViewModel = new ConnectToGroupViewModel(m_dataService, LoadData);
            CreateNewGroupViewModel = new CreateGroupViewModel(m_dataService, Navigate);
            DeleteGroupViewModel = new DeleteGroupViewModel(m_dataService, m_selectedGroups, LoadData);

            SwitchToPauseViewModel = new SwitchGroupStateViewModel(GroupStateContract.Paused, m_dataService, m_selectedGroups, LoadData);
            SwitchToRunningViewModel = new SwitchGroupStateViewModel(GroupStateContract.Running, m_dataService, m_selectedGroups, LoadData);
        }

        private void Navigate(Type type)
        {
            m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);
            m_navigationService.Navigate(type);
        }

        private void GoBack()
        {
            m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);
            m_navigationService.GoBack();
        }

        private void LoadData()
        {
            m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);
            Loading = true;

            m_dataService.GetGroupList((groupList, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                m_groups = groupList;
                DisplayGroupList(m_groups);

                m_pollingService.RegisterForGroupsUpdate(UpdatePollingInterval, m_groups, GroupUpdate);
            });

            m_dataService.GetLoggedUserInfo(false, info =>
            {
                m_userRole = info.UserRole;
                RaisePropertyChanged(() => IsTeacherMode);
                RaisePropertyChanged(() => IsTeacherAndGroupSelected);
            });
        }

        private void GroupUpdate(Exception exception)
        {
        }

        private void GroupClick(ItemClickEventArgs args)
        {
            var group = args.ClickedItem as GroupInfoViewModel;
            OpenGroup(group);
        }

        private void OpenGroup(GroupInfoViewModel group)
        {
            if (group != null)
            {
                m_dataService.SetCurrentGroup(group.GroupId);
                m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);

                var viewType = group.GroupType == GroupType.Member
                    ? typeof (ApplicationHostView)
                    : typeof (GroupPageView);
                Navigate(viewType);
                Messenger.Default.Unregister(this);
            }
        }

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
            IsOneItemSelected = m_selectedGroups.Count == 1;
            DeleteGroupViewModel.SelectedGroupCount = m_selectedGroups.Count;

            OnlyOwnedGroupsSelected = m_selectedGroups.All(group => group.GroupType == GroupType.Owner);
        }

        private IEnumerable<GroupInfoViewModel> GetSortedGroupList(IEnumerable<GroupInfoViewModel> list)
        {
            switch (SelectedSortType)
            {
                case SortGroupItem.SortType.CreateTime:
                    return list.OrderByDescending(model => model.CreateTime);
                case SortGroupItem.SortType.Name:
                    return list.OrderBy(model => model.GroupName);
                case SortGroupItem.SortType.State:
                    return list.OrderBy(model => model.State);
                default:
                    return list;
            }
        }

        private void DisplayGroupList(ObservableCollection<GroupInfoViewModel> groupList)
        {
            IEnumerable<GroupInfoViewModel> filteredList = groupList;
            if (CurrentFilter != null)
                filteredList = groupList.Where(group => group.State == CurrentFilter);

            filteredList = GetSortedGroupList(filteredList);

            var groupedList = filteredList.GroupBy(group => group.GroupType).OrderBy(group => group.Key);
            GroupList = new ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>>(groupedList);
        }

        private void Filter(object state)
        {
            if (state == null)
            {
                IsFilter = false;
                CurrentFilter = null;
            }
            else
            {
                IsFilter = true;
                CurrentFilter = state is short ? (GroupStateContract)state : 0;
            }

            DisplayGroupList(m_groups);
        }
    }
}