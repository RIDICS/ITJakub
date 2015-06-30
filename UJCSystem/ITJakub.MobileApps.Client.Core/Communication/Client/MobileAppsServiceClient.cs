using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataContracts.Tasks;

namespace ITJakub.MobileApps.Client.Core.Communication.Client
{
    public class MobileAppsServiceClient : ClientBase<IMobileAppsService>
    {
        //private const string EndpointAddress = "http://localhost/ITJakub.MobileApps.Service/MobileAppsService.svc";
        //private const string EndpointAddress = "http://147.32.81.136/ITJakub.MobileApps.Service/MobileAppsService.svc";
        private const string EndpointAddress = "http://itjakubmobileapps.cloudapp.net/MobileAppsService.svc";

        private readonly ClientMessageInspector m_clientMessageInspector;

        public MobileAppsServiceClient() : base(GetDefaultBinding(), GetDefaultEndpointAddress())
        {
            m_clientMessageInspector = new ClientMessageInspector();
            var endpointBehavior = new CustomEndpointBehavior(m_clientMessageInspector);
            Endpoint.EndpointBehaviors.Add(endpointBehavior);
        }

        public void UpdateCommunicationToken(string communicationToken)
        {
            m_clientMessageInspector.CommunicationToken = communicationToken;
        }

        public Task CreateUserAsync(AuthProvidersContract providerContract, string providerToken,
            UserDetailContract userDetail)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.CreateUser(providerContract, providerToken, userDetail);
                }
                catch (FaultException ex)
                {
                    throw new UserAlreadyRegisteredException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<LoginUserResponse> LoginUserAsync(AuthProvidersContract providerContract, string providerToken, string email)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.LoginUser(providerContract, providerToken, email);
                }
                catch (FaultException ex)
                {
                    throw new UserNotRegisteredException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<UserGroupsContract> GetGroupsByUserAsync(long userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetGroupsByUser(userId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<CreateGroupResponse> CreateGroupAsync(long userId, string groupName)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.CreateGroup(userId, groupName);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task AddUserToGroupAsync(string groupAccessCode, long userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.AddUserToGroup(groupAccessCode, userId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException("Invalid server operation, probably wrong access code or group is unopened or closed.", ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task CreateSynchronizedObjectAsync(int applicationId, long groupId, long userId, SynchronizedObjectContract synchronizedObject)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.CreateSynchronizedObject(applicationId, groupId, userId, synchronizedObject);
                }
                catch (FaultException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<IList<SynchronizedObjectResponseContract>> GetSynchronizedObjectsAsync(long groupId, int applicationId, string objectType, DateTime since)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetSynchronizedObjects(groupId, applicationId, objectType, since);
                }
                catch (FaultException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<SynchronizedObjectResponseContract> GetLatestSynchronizedObjectAsync(long groupId, int applicationId, string objectType, DateTime since)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetLatestSynchronizedObject(groupId, applicationId, objectType, since);
                }
                catch (FaultException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<IList<ApplicationContract>> GetAllApplicationAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetAllApplication();
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<GroupDetailContract> GetGroupDetailsAsync(long groupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetGroupDetails(groupId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }
        
        public Task AssignTaskToGroupAsync(long groupId, long taskId)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.AssignTaskToGroup(groupId, taskId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<IList<TaskDetailContract>> GetTasksByApplicationAsync(int applicationId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetTasksByApplication(applicationId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<IList<TaskDetailContract>> GetTasksByAuthor(long userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetTasksByAuthor(userId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task CreateTaskAsync(long userId, int applicationId, string name, string data)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.CreateTask(userId, applicationId, name, data);
                }
                catch (FaultException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<TaskDataContract> GetTaskForGroupAsync(long groupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetTaskForGroup(groupId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<IList<GroupDetailsUpdateContract>> GetGroupsUpdateAsync(IList<OldGroupDetailsContract> groups)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetGroupsUpdate(groups);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task UpdateGroupStateAsync(long groupId, GroupStateContract state)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.UpdateGroupState(groupId, state);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task RemoveGroupAsync(long groupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    Channel.RemoveGroup(groupId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        public Task<GroupStateContract> GetGroupStateAsync(long groupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetGroupState(groupId);
                }
                catch (FaultException ex)
                {
                    throw new InvalidServerOperationException(ex);
                }
                catch (CommunicationException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (TimeoutException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    throw new ClientCommunicationException(ex);
                }
            });
        }

        #region enpoint settings
        private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBindingIMobileAppsService))
            {
                var result = new BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.",
                endpointConfiguration));
        }

        private static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBindingIMobileAppsService))
            {
                return new EndpointAddress(EndpointAddress);
            }
            throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.",
                endpointConfiguration));
        }

        private static Binding GetDefaultBinding()
        {
            return
                GetBindingForEndpoint(EndpointConfiguration.BasicHttpBindingIMobileAppsService);
        }

        private static EndpointAddress GetDefaultEndpointAddress()
        {
            return GetEndpointAddress(EndpointConfiguration.BasicHttpBindingIMobileAppsService);
        }
        #endregion
    }
}