using System.Net.Http;
using System.Threading.Tasks;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.DataContracts;

namespace Vokabular.Authentication.Client.Client.Auth
{
    public class NonceApiClient : BaseApiClient
    {
        public NonceApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.NonceBasePath;

        public async Task<NonceContract> CreateAsync(
            int userId,
            NonceTypeEnum nonceType
        )
        {
            var nonceContract = new NonceContract
            {
                UserId = userId,
                Type = nonceType,
            };

            using (var nonceResponse =
                await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, $"{BasePath}create", nonceContract))
            {
                return await m_authorizationServiceHttpClient.GetDeserializedModelAsync<NonceContract>(nonceResponse);
            }
        }
    }
}