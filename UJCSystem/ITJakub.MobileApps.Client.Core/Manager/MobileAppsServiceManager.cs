using System;
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

        public MobileAppsServiceManager()
        {
            m_serviceClient = new MobileAppsServiceClient();
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
                    EstimatedExpirationTime = response.EstimatedExpirationTime
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
                var authenticationProvider = ConvertLoginProviderToString(loginProviderType);

                await m_serviceClient.CreateUserAsync(authenticationProvider, userInfo.AccessToken, new User
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
    }
}
