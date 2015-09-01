using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class AdminHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private readonly IMainPollingService m_pollingService;
        private string m_groupName;
        private AdminBaseViewModel m_adminViewModel;
        private bool m_isCommandBarOpen;
        private bool m_isChatSupported;
        private bool m_isChatDisplayed;
        private SupportAppBaseViewModel m_chatApplicationViewModel;
        private GroupInfoViewModel m_groupInfo;
        private const PollingInterval GroupMembersPollingInterval = PollingInterval.Medium;

        public AdminHostViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;
            m_pollingService = pollingService;
            InitCommands();
            LoadData();
        }
        
        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void GoBack()
        {
            m_pollingService.UnregisterAll();
            m_navigationService.GoBack();
        }

        private void LoadData()
        {
            m_dataService.GetCurrentGroupId((groupId, type) =>
            {
                LoadGroupDetails(groupId);
            });
        }

        private void LoadGroupDetails(long groupId)
        {
            m_groupInfo = new GroupInfoViewModel
            {
                GroupId = groupId,
                GroupType = GroupType.Owner
            };

            m_dataService.GetGroupDetails(groupId, (groupInfo, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                GroupName = groupInfo.GroupName;
            });

            m_dataService.GetTaskForGroup(groupId, (taskInfo, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowConnectionError(GoBack);
                    return;
                }

                LoadApps(taskInfo.Application, taskInfo.Data);
            });
        }
        
        private void LoadApps(ApplicationType application, string data)
        {
            if (data == null)
            {
                m_errorService.ShowError("Nebyla přijata žádná data potřebná pro zobrazení aplikace s konkrétním zadáním (úlohou). Aplikace byla ukončena.", "Nepřijata žádná data");
                GoBack();
                return;
            }

            m_dataService.GetApplicationByTypes(new [] {application, ApplicationType.Chat}, (apps, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowError("Tato skupina obsahuje neznámé zadání. Pro otevření této skupiny použijte jinou aplikaci.", "Neznámá aplikace", GoBack);
                    return;
                }

                var currentApplication = apps[application];
                var chatApplication = apps[ApplicationType.Chat];

                AdminViewModel = currentApplication.AdminViewModel;
                IsChatSupported = currentApplication.IsChatSupported;
                AdminViewModel.SetTask(data);
                AdminViewModel.InitializeCommunication();

                if (IsChatSupported)
                {
                    ChatApplicationViewModel = chatApplication.ApplicationViewModel as SupportAppBaseViewModel;
                    if (ChatApplicationViewModel != null)
                    {
                        ChatApplicationViewModel.InitializeCommunication();
                    }
                }

                m_pollingService.RegisterForGroupsUpdate(GroupMembersPollingInterval, new[] { m_groupInfo }, GroupMembersUpdated);
            });
        }

        private void GroupMembersUpdated(Exception error)
        {
            if (error != null)
            {
                m_errorService.ShowConnectionWarning();
            }
            else
            {
                var members = m_groupInfo.Members.Select(x => new UserInfo
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                });
                DispatcherHelper.CheckBeginInvokeOnUI(() => AdminViewModel.UpdateGroupMembers(members));
            }
        }

        public RelayCommand GoBackCommand { get; private set; }

        public string GroupName
        {
            get { return m_groupName; }
            set
            {
                m_groupName = value;
                RaisePropertyChanged();
            }
        }

        public AdminBaseViewModel AdminViewModel
        {
            get { return m_adminViewModel; }
            set
            {
                m_adminViewModel = value;
                RaisePropertyChanged();
            }
        }

        public SupportAppBaseViewModel ChatApplicationViewModel
        {
            get { return m_chatApplicationViewModel; }
            set
            {
                m_chatApplicationViewModel = value;
                RaisePropertyChanged();
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

        public bool IsChatSupported
        {
            get { return m_isChatSupported; }
            set
            {
                m_isChatSupported = value;
                RaisePropertyChanged();
            }
        }

        public bool IsChatDisplayed
        {
            get { return m_isChatDisplayed; }
            set
            {
                m_isChatDisplayed = value;
                RaisePropertyChanged();
            }
        }
    }
}
