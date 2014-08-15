using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Manager.Groups
{
    public class GroupManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly AuthenticationManager m_authManager;
        private readonly UserAvatarCache m_userAvatarCache;
        private readonly BitmapImage m_defaultUserAvatar;

        public long CurrentGroupId { get; private set; }

        public GroupManager(MobileAppsServiceClient serviceClient, AuthenticationManager authManager, UserAvatarCache userAvatarCache)
        {
            m_serviceClient = serviceClient;
            m_authManager = authManager;
            m_userAvatarCache = userAvatarCache;
            m_defaultUserAvatar = new BitmapImage(new Uri("ms-appx:///Icon/user-32.png"));

            //TODO for debug
            CurrentGroupId = 1;
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
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Member,
                        Members = new ObservableCollection<GroupMemberViewModel>(),
                        //TODO Hack for debug
                        ApplicationType = ApplicationType.SampleApp
                    };
                    FillGroupMembers(newGroup, groupDetails.Members);
                    list.Add(newGroup);
                }

                foreach (var groupDetails in response.OwnedGroups)
                {
                    var newGroup = new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Owner,
                        GroupCode = groupDetails.EnterCode,
                        CreateTime = groupDetails.CreateTime,
                        Members = new ObservableCollection<GroupMemberViewModel>()
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
                group.Members.Add(new GroupMemberViewModel
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Id = member.Id,
                    UserAvatar = m_defaultUserAvatar
                });
                m_userAvatarCache.AddAvatarUrl(member.Id, member.AvatarUrl);
            }
        }

        public async void CreateNewGroup(string groupName, Action<CreateGroupResponse, Exception> callback)
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
                callback(result, null);
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

        public async void OpenGroupAndGetDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            try
            {
                CurrentGroupId = groupId;
                await m_serviceClient.GetGroupDetails(groupId);
                callback(new GroupInfoViewModel(), null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public void LoadGroupMemberAvatars(IList<GroupMemberViewModel> groupMembers)
        {
            foreach (var member in groupMembers)
            {
                LoadMemberAvatar(member);
            }
        }

        private async void LoadMemberAvatar(GroupMemberViewModel member)
        {
            member.UserAvatar = await m_userAvatarCache.GetUserAvatar(member.Id);
        }

        public async void UpdateGroupMembers(GroupInfoViewModel group)
        {
            var oldMembersId = new HashSet<long>(group.Members.Select(member => member.Id));
            var newMembersId = await m_serviceClient.GetGroupMemberIdsAsync(group.GroupId);
            if (oldMembersId.Count != newMembersId.Count)
            {
                //TODO load group details
                return;
            }
            foreach (var newMemberId in newMembersId)
            {
                if (!oldMembersId.Contains(newMemberId))
                {
                    //TODO load group details
                }
            }
        }

        private async void UpdateGroupMembers2(GroupInfoViewModel group)
        {
            var members = await m_serviceClient.GetGroupMembersAsync(group.GroupId);
            group.Members = new ObservableCollection<GroupMemberViewModel>(members.Select(groupContract => new GroupMemberViewModel
            {
                FirstName = groupContract.FirstName,
                LastName = groupContract.LastName,
                Id = groupContract.Id,
                UserAvatar = m_defaultUserAvatar
            }));
        }
    }
}