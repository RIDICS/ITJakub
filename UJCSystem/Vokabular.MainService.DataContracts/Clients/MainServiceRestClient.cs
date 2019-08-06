using System.Net.Http;
using System.Net.Http.Headers;
using Vokabular.RestClient;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRestClient : FullRestClientBase
    {
        private readonly IMainServiceAuthTokenProvider m_tokenProvider;
        private const string AuthenticationScheme = "Bearer";

        public MainServiceRestClient(IMainServiceUriProvider uriProvider, IMainServiceAuthTokenProvider tokenProvider) : base(uriProvider.MainServiceUri)
        {
            m_tokenProvider = tokenProvider;
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, m_tokenProvider.AuthToken);
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }
    }
}