using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Groups
{
    public class GroupManager
    {
        private readonly MobileAppsServiceManager m_serviceManager;
        private readonly AuthenticationManager m_authManager;

        public GroupManager(MobileAppsServiceManager serviceManager, AuthenticationManager authManager)
        {
            m_serviceManager = serviceManager;
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

                ObservableCollection<GroupInfoViewModel> list = await m_serviceManager.GetGroupListAsync(userId.Value);
                callback(list, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void CreateNewGroup(string groupName, Action<CreateGroupResult, Exception> callback)
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
                CreateGroupResult result = await m_serviceManager.CreateGroupAsync(userId.Value, groupName);
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

                await m_serviceManager.AddUserToGroupAsync(code, userId.Value);
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
                var result = await m_serviceManager.GetGroupDetailsAsync(groupId);
                callback(result, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}