using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.ViewModel.ComboBoxItem;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public abstract class GroupListViewModelBase : ViewModelBase
    {
        protected const PollingInterval UpdatePollingInterval = PollingInterval.Medium;
        private ObservableCollection<GroupInfoViewModel> m_groupList;
        protected List<GroupInfoViewModel> m_completeGroupList;
        private bool m_isGroupListEmpty;
        private bool m_loading;
        private GroupStateContract? m_currentFilter;
        private SortGroupItem.SortType m_selectedSortType;

        protected GroupListViewModelBase()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            GroupClickCommand = new RelayCommand<ItemClickEventArgs>(OpenGroup);
            OpenGroupCommand = new RelayCommand<GroupInfoViewModel>(OpenGroup);
            FilterCommand = new RelayCommand<object>(Filter);
        }

        protected abstract void OpenGroup(GroupInfoViewModel group);

        public abstract bool IsTeacherView { get; }

        public RelayCommand<ItemClickEventArgs> GroupClickCommand { get; set; }

        public RelayCommand<object> FilterCommand { get; private set; }

        public RelayCommand<GroupInfoViewModel> OpenGroupCommand { get; private set; }

        public int DefaultIndex
        {
            get { return 0; }
        }

        public Visibility TeacherControlVisibility
        {
            get { return IsTeacherView ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<GroupInfoViewModel> GroupList
        {
            get { return m_groupList; }
            set
            {
                m_groupList = value;
                RaisePropertyChanged();
                IsGroupListEmpty = m_groupList.Count == 0;
            }
        }

        public bool IsGroupListEmpty
        {
            get { return m_isGroupListEmpty; }
            set
            {
                m_isGroupListEmpty = value;
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
                DisplayGroupList();
            }
        }


        private void OpenGroup(ItemClickEventArgs args)
        {
            var group = args.ClickedItem as GroupInfoViewModel;
            OpenGroup(group);
        }

        private void Filter(object state)
        {
            if (state == null)
            {
                CurrentFilter = null;
            }
            else
            {
                CurrentFilter = (GroupStateContract)Convert.ToInt16(state);
            }

            DisplayGroupList();
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

        private IEnumerable<GroupInfoViewModel> GetFilteredGroupList(IEnumerable<GroupInfoViewModel> list)
        {
            if (CurrentFilter != null)
                return list.Where(group => group.State == CurrentFilter);

            return list;
        }

        public void DisplayGroupList()
        {
            if (m_completeGroupList == null)
            {
                GroupList = new ObservableCollection<GroupInfoViewModel>();
                return;
            }

            var filteredList = GetFilteredGroupList(m_completeGroupList);
            var sortedList = GetSortedGroupList(filteredList);

            GroupList = new ObservableCollection<GroupInfoViewModel>(sortedList);
        }
    }
}
