using System.Threading.Tasks;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.Client.Contract;

namespace Vokabular.Authentication.Client.Client.Auth
{
    public class FileResourceApiClient : BaseApiClient
    {
        public FileResourceApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.FileResourceBasePath;

        public async Task<FileResourceStreamContract> GetAsync(int id)
        {
            return await m_authorizationServiceHttpClient
                .GetItemStreamAsync<FileResourceStreamContract, FileResourceStreamHydrator<FileResourceStreamContract>>(BasePath, id);
        }
    }
}