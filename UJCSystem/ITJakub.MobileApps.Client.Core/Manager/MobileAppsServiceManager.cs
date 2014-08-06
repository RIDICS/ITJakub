using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Converter;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class MobileAppsServiceManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly ClientMessageInspector m_clientMessageInspector;

        public MobileAppsServiceManager()
        {
            m_serviceClient = new MobileAppsServiceClient();
            m_clientMessageInspector = new ClientMessageInspector();
            var endpointBehavior = new CustomEndpointBehavior(m_clientMessageInspector);
            m_serviceClient.Endpoint.EndpointBehaviors.Add(endpointBehavior);
        }

        public void UpdateCommunicationToken(string communicationToken)
        {
            m_clientMessageInspector.CommunicationToken = communicationToken;
        }

        public async Task<LoginResult> LoginUserAsync(LoginProviderType loginProviderType, string email, string accessToken)
        {
            try
            {
                var authenticationProvider = LoginProviderConverter.LoginToAuthenticationProvider(loginProviderType);
                var response = await m_serviceClient.LoginUserAsync(new UserLogin
                {
                    AuthenticationProvider = authenticationProvider,
                    AuthenticationToken = accessToken,
                    Email = email
                });
                var loginResult = new LoginResult
                {
                    CommunicationToken = response.CommunicationToken,
                    EstimatedExpirationTime = response.EstimatedExpirationTime,
                    UserId = response.UserId
                };
                return loginResult;
            }
            catch (FaultException)
            {
                throw new UserNotRegisteredException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task CreateUser(LoginProviderType loginProviderType, UserInfo userInfo)
        {
            try
            {
                var authenticationProvider = LoginProviderConverter.LoginToAuthenticationProvider(loginProviderType);
                var providerString = ((byte) authenticationProvider).ToString();

                await m_serviceClient.CreateUserAsync(providerString, userInfo.AccessToken, new User
                {
                    Email = userInfo.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName
                });
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task<ObservableCollection<GroupInfoViewModel>> GetGroupListAsync(long userId)
        {
            try {
                var response = await m_serviceClient.GetMembershipsForUserAsync(userId.ToString());
                var list = new ObservableCollection<GroupInfoViewModel>();
                foreach (var groupDetails in response)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Group.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Member
                    });
                }
                response = await m_serviceClient.GetGroupsByUserAsync(userId.ToString());
                foreach (var groupDetails in response)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Group.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Owner
                    });
                }
                return list;
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task<CreateGroupResult> CreateGroupAsync(long userId, string groupName)
        {
            try
            {
                var response = await m_serviceClient.CreateGroupAsync(userId.ToString(), groupName);
                var result = new CreateGroupResult
                {
                    EnterCode = response.EnterCode
                };
                return result;
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task AddUserToGroupAsync(string code, long userId)
        {
            try
            {
                await m_serviceClient.AddUserToGroupAsync(code, userId.ToString());
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }
    }
}
