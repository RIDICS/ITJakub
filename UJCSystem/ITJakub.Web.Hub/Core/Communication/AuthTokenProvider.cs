using Ridics.Core.HttpClient.Provider;
using Vokabular.MainService.DataContracts;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class AuthTokenProvider : IMainServiceAuthTokenProvider
    {
        private readonly AuthApiAccessTokenProvider m_authApiAccessTokenProvider;

        public AuthTokenProvider(AuthApiAccessTokenProvider authApiAccessTokenProvider)
        {
            m_authApiAccessTokenProvider = authApiAccessTokenProvider;
        }

        public string AuthToken => m_authApiAccessTokenProvider.GetAccessTokenAsync().GetAwaiter().GetResult();
    }
}
