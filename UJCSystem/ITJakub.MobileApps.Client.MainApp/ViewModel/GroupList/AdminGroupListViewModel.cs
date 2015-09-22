using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class AdminGroupListViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private ObservableCollection<GroupInfoViewModel> m_groupList;
        private List<GroupInfoViewModel> m_selectedGroups;
        private bool m_isGroupListEmpty;
        private bool m_loading;

        public AdminGroupListViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            m_selectedGroups = new List<GroupInfoViewModel>();

            InitCommands();
            LoadData();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            GroupClickCommand = new RelayCommand<ItemClickEventArgs>(OpenGroup);
            SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(SelectionChanged);
        }
        
        private void LoadData()
        {
            Loading = true;
            m_dataService.GetOwnedGroupsForCurrentUser((groupList, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionWarning();
                    return;
                }

                GroupList = new ObservableCollection<GroupInfoViewModel>(groupList);
            });
        }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand<ItemClickEventArgs> GroupClickCommand { get; set; }

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }

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

        private void OpenGroup(ItemClickEventArgs args)
        {
            var group = args.ClickedItem as GroupInfoViewModel;
            if (group == null)
                return;

            m_dataService.SetCurrentGroup(group.GroupId, GroupType.Owner);
            m_navigationService.Navigate<GroupPageView>();
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
            //IsOneItemSelected = m_selectedGroups.Count == 1;
            //DeleteGroupViewModel.SelectedGroupCount = m_selectedGroups.Count;

            UpdateGroupStateActions();
        }

        private void UpdateGroupStateActions()
        {
            // Enable only appropriate group state change
            //if (m_selectedGroups.Any(model => model.GroupType == GroupType.Member))
            //{
            //    CanPauseSelected = false;
            //    CanStartSelected = false;
            //    CanRemoveSelected = false;
            //    return;
            //}

            //var stateCount = m_selectedGroups.GroupBy(model => model.State).ToDictionary(models => models.Key, models => models.Count());
            //var totalCount = m_selectedGroups.Count();

            //for (var state = GroupStateContract.Created; state <= GroupStateContract.Closed; state++)
            //{
            //    if (!stateCount.ContainsKey(state))
            //        stateCount[state] = 0;
            //}
            //CanPauseSelected = stateCount[GroupStateContract.Running] == totalCount;
            //CanRemoveSelected = stateCount[GroupStateContract.Created] + stateCount[GroupStateContract.Closed] == totalCount;
            //CanStartSelected = stateCount[GroupStateContract.WaitingForStart] + stateCount[GroupStateContract.Paused] == totalCount;
        }
    }
}
