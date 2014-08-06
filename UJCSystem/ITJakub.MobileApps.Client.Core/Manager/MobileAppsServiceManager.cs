using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class MobileAppsServiceManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private ClientMessageInspector m_clientMessageInspector;

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
                var response = await m_serviceClient.LoginUserAsync(new UserLogin
                {
                    AuthenticationProvider = ConvertLoginToAuthenticationProvider(loginProviderType),
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
                var authenticationProvider = ConvertLoginToAuthenticationProvider(loginProviderType);

                await m_serviceClient.CreateUserAsync(userInfo.AccessToken,authenticationProvider, new User
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

        private AuthenticationProviders ConvertLoginToAuthenticationProvider(LoginProviderType loginProviderType)
        {
            AuthenticationProviders provider;
            switch (loginProviderType)
            {
                case LoginProviderType.LiveId:
                    provider = AuthenticationProviders.LiveId;
                    break;
                case LoginProviderType.Google:
                    provider = AuthenticationProviders.Google;
                    break;
                case LoginProviderType.Facebook:
                    provider = AuthenticationProviders.Facebook;
                    break;
                default:
                    provider = AuthenticationProviders.ItJakub;
                    break;
            }
            return provider;
        }

        private string ConvertLoginProviderToString(LoginProviderType loginProviderType)
        {
            var provider = ConvertLoginToAuthenticationProvider(loginProviderType);
            return ((byte)provider).ToString();
        }

        public async Task<ObservableCollection<GroupInfoViewModel>> GetGroupListAsync(string userId)
        {
            try {
                var response = await m_serviceClient.GetMembershipsForUserAsync(userId);
                var list = new ObservableCollection<GroupInfoViewModel>();
                foreach (var groupDetails in response)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Group.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
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
    }
}
