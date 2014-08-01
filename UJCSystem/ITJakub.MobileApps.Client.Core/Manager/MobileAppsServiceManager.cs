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

        public async Task<LoginResult> LoginUserAsync(LoginProvider loginProvider, string email, string accessToken)
        {
            try
            {
                var response = await m_serviceClient.LoginUserAsync(new UserLogin
                {
                    AuthenticationProvider = ConvertLoginToAuthenticationProvider(loginProvider),
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

        public async Task CreateUser(LoginProvider loginProvider, UserInfo userInfo)
        {
            try
            {
                var authenticationProvider = ConvertLoginProviderToString(loginProvider);

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

        private AuthenticationProviders ConvertLoginToAuthenticationProvider(LoginProvider loginProvider)
        {
            AuthenticationProviders provider;
            switch (loginProvider)
            {
                case LoginProvider.LiveId:
                    provider = AuthenticationProviders.LiveId;
                    break;
                case LoginProvider.Google:
                    provider = AuthenticationProviders.Google;
                    break;
                case LoginProvider.Facebook:
                    provider = AuthenticationProviders.Facebook;
                    break;
                default:
                    provider = AuthenticationProviders.ItJakub;
                    break;
            }
            return provider;
        }

        private string ConvertLoginProviderToString(LoginProvider loginProvider)
        {
            var provider = ConvertLoginToAuthenticationProvider(loginProvider);
            return ((byte)provider).ToString();
        }
    }
}
