using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Vokabular.RestClient;
using Vokabular.RestClient.Contracts;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRestClient : FullRestClient
    {
        private readonly IMainServiceAuthTokenProvider m_tokenProvider;
        private readonly IMainServiceClientLocalization m_localization;
        private const string AuthenticationScheme = "Bearer";

        public MainServiceRestClient(MainServiceClientConfiguration configuration, IMainServiceAuthTokenProvider tokenProvider,
            IMainServiceClientLocalization localization) : base(configuration)
        {
            m_tokenProvider = tokenProvider;
            m_localization = localization;
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, m_tokenProvider.AuthToken);
        }

        protected override void TryParseAndThrowResponseError(HttpStatusCode responseStatusCode, string responseContent)
        {
            if (responseStatusCode == HttpStatusCode.BadRequest &&
                TryDeserialize<ErrorContract>(responseContent, out var errorContract))
            {
                var exception = new MainServiceException(errorContract.Code, errorContract.Description, responseStatusCode, errorContract.DescriptionParams);

                m_localization.LocalizeApiException(exception);

                throw exception;
            }
        }
    }
}