using Vokabular.Authentication.Client.Configuration;

namespace Vokabular.Authentication.Client.Client.Auth
{
    public abstract class BaseApiClient
    {
        protected const int DefaultListCount = 20;

        protected readonly AuthorizationServiceHttpClient m_authorizationServiceHttpClient;

        protected readonly AuthServiceControllerBasePathsConfiguration m_basePathsConfiguration;

        protected BaseApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        )
        {
            m_authorizationServiceHttpClient = authorizationServiceHttpClient;
            m_basePathsConfiguration = basePathsConfiguration;
        }

        protected abstract string BasePath { get; }
    }
}