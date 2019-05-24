using System.Threading.Tasks;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.DataContracts;

namespace Vokabular.Authentication.Client.Client.Auth
{
    public class ExternalIdentityProviderApiClient : BaseApiClient
    {
        public ExternalIdentityProviderApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.ExternalLoginProviderBasePath;

        public async Task<ListContract<ExternalLoginProviderContract>> ListExternalIdentityProviderAsync(
            int start = 0, int count = DefaultListCount
        )
        {
            return await m_authorizationServiceHttpClient.GetListAsync<ExternalLoginProviderContract>(start, count);
        }
    }
}