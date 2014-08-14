using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Manager.Groups
{
    public class GroupManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly AuthenticationManager m_authManager;

        public long? CurrentGroupId { get; set; }

        public GroupManager(MobileAppsServiceClient serviceClient, AuthenticationManager authManager)
        {
            m_serviceClient = serviceClient;
            m_authManager = authManager;
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
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Member,
                        //TODO Hack for debug
                        ApplicationType = ApplicationType.SampleApp
                    });
                }

                foreach (var groupDetails in response.OwnedGroups)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Owner,
                        GroupCode = groupDetails.EnterCode,
                        CreateTime = groupDetails.CreateTime
                    });
                }
                
                callback(list, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
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

        public async void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            try
            {
                //TODO pokud bude tado metoda potreba, tak doimplementovat server i klienta
                
                callback(new GroupInfoViewModel(), null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}