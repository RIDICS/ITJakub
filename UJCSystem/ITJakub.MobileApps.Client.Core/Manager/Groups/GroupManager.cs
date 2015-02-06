using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Comparer;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core.Manager.Groups
{
    public class GroupManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly AuthenticationManager m_authManager;
        private readonly UserAvatarCache m_userAvatarCache;
        private readonly BitmapImage m_defaultUserAvatar;
        private readonly ApplicationIdManager m_applicationIdManager;
        private GroupDetailContract m_currentGroupInfoModel;

        public long CurrentGroupId { get; private set; }

        public bool RestoreLastState { get; set; }

        public GroupManager(IUnityContainer container)
        {
            m_serviceClient = container.Resolve<MobileAppsServiceClient>();
            m_authManager = container.Resolve<AuthenticationManager>();
            m_userAvatarCache = container.Resolve<UserAvatarCache>();
            m_applicationIdManager = container.Resolve<ApplicationIdManager>();

            m_defaultUserAvatar = new BitmapImage(new Uri("ms-appx:///Icon/user-32.png"));
        }

        public async void GetGroupForCurrentUser(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            try
            {
                var userId = m_authManager.GetCurrentUserId();
                if (!userId.HasValue)
                {
                    callback(null, new ArgumentException("No logged user"));
                    return;
                }

                var response = await m_serviceClient.GetGroupsByUserAsync(userId.Value);
                var list = new ObservableCollection<GroupInfoViewModel>();

                foreach (var groupDetails in response.MemberOfGroup)
                {
                    var newGroup = new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Member,
                        State = groupDetails.State,
                        CreateTime = groupDetails.CreateTime,
                        Members = new ObservableCollection<GroupMemberViewModel>(),
                        Task = new TaskViewModel()
                    };
                    FillGroupMembers(newGroup, groupDetails.Members);
                    list.Add(newGroup);
                }

                foreach (var groupDetails in response.OwnedGroups)
                {
                    var newGroup = new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Owner,
                        State = groupDetails.State,
                        GroupCode = groupDetails.EnterCode,
                        CreateTime = groupDetails.CreateTime,
                        Members = new ObservableCollection<GroupMemberViewModel>(),
                        Task = new TaskViewModel()
                    };
                    FillGroupMembers(newGroup, groupDetails.Members);
                    list.Add(newGroup);
                }
                
                callback(list, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        private void FillGroupMembers(GroupInfoViewModel group, IEnumerable<GroupMemberContract> members)
        {
            foreach (var member in members)
            {
                var memberViewModel = new GroupMemberViewModel
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Id = member.Id,
                    UserAvatar = m_defaultUserAvatar
                };

                group.Members.Add(memberViewModel);
                
                m_userAvatarCache.AddAvatarUrl(member.Id, member.AvatarUrl);
                LoadMemberAvatar(memberViewModel);
            }

            group.MemberCount = group.Members.Count;
            group.Members = new ObservableCollection<GroupMemberViewModel>(group.Members.OrderBy(model => model, new GroupMemberComparer()));
        }

        public async void CreateNewGroup(string groupName, Action<CreatedGroupViewModel, Exception> callback)
        {
            try
            {
                var userId = m_authManager.GetCurrentUserId();
                if (!userId.HasValue)
                {
                    callback(null, new ArgumentException("No logged user"));
                    return;
                }

                //TODO check groupName validity
                var result = await m_serviceClient.CreateGroupAsync(userId.Value, groupName);
                var viewModel = new CreatedGroupViewModel
                {
                    EnterCode = result.EnterCode,
                    GroupId = result.GroupId
                };

                callback(viewModel, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void ConnectToGroup(string code, Action<Exception> callback)
        {
            try
            {

                var userId = m_authManager.GetCurrentUserId();
                if (!userId.HasValue)
                {
                    callback(new ArgumentException("No logged user"));
                    return;
                }

                await m_serviceClient.AddUserToGroupAsync(code, userId.Value);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            try
            {
                CurrentGroupId = groupId; // TODO

                if (!RestoreLastState)
                {
                    m_currentGroupInfoModel = null;
                    m_currentGroupInfoModel = await m_serviceClient.GetGroupDetailsAsync(groupId);
                }

                var groupInfo = m_currentGroupInfoModel;
                var group = new GroupInfoViewModel
                {
                    GroupId = groupInfo.Id,
                    Members = new ObservableCollection<GroupMemberViewModel>(),
                    GroupName = groupInfo.Name,
                    CreateTime = groupInfo.CreateTime,
                    GroupCode = groupInfo.EnterCode,
                    State = groupInfo.State,
                };

                var task = groupInfo.Task;
                if (task != null)
                {
                    group.Task = new TaskViewModel
                    {
                        Application = m_applicationIdManager.GetApplicationType(task.ApplicationId),
                        Id = task.Id,
                        Name = task.Name,
                        CreateTime = task.CreateTime
                    };
                }
                
                FillGroupMembers(group, groupInfo.Members);
                callback(group, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
        
        private async void LoadMemberAvatar(GroupMemberViewModel member)
        {
            member.UserAvatar = await m_userAvatarCache.GetUserAvatar(member.Id);
        }

        public async Task UpdateGroupsMembersAsync(IList<GroupInfoViewModel> groups, Action<Exception> callback)
        {
            try
            {
                var oldGroupInfo = groups.Select(group => new OldGroupDetailsContract
                {
                    Id = group.GroupId,
                    MemberIds = group.Members.Select(x => x.Id).ToList()
                }).ToList();

                var result = await m_serviceClient.GetGroupsUpdateAsync(oldGroupInfo);
                if (result.Count == 0)
                    return;

                var groupUpdate = result.ToDictionary(group => group.Id, group => group);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foreach (var group in groups.Where(group => groupUpdate.ContainsKey(group.GroupId)))
                    {
                        FillGroupMembers(group, groupUpdate[group.GroupId].Members);
                    }
                    callback(null);
                });
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public void OpenGroup(long groupId)
        {
            CurrentGroupId = groupId;
        }

        public async void UpdateGroupState(long groupId, GroupStateContract newState, Action<Exception> callback)
        {
            try
            {
                await m_serviceClient.UpdateGroupStateAsync(groupId, newState);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async void RemoveGroup(long groupId, Action<Exception> callback)
        {
            try
            {
                await m_serviceClient.RemoveGroupAsync(groupId);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async Task GetGroupStateAsync(long groupId, Action<GroupStateContract, Exception> callback)
        {
            try
            {
                var state = await m_serviceClient.GetGroupStateAsync(groupId);
                callback(state, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(GroupStateContract.Closed, exception);
            }
        }
    }
}