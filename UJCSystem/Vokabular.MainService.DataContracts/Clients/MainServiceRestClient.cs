using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Vokabular.RestClient;
using Vokabular.RestClient.Contracts;
using Vokabular.RestClient.Extensions;

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

        protected override void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var responseStatusCode = response.StatusCode;
            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (responseStatusCode == HttpStatusCode.BadRequest &&
                TryDeserializeErrorResult(responseContent, out var errorContract))
            {
                var exception = new MainServiceException(errorContract.Code, errorContract.Description, responseStatusCode, errorContract.DescriptionParams);

                m_localization.LocalizeApiException(exception);

                throw exception;
            }
    
            base.EnsureSuccessStatusCode(response);
        }

        private bool TryDeserializeErrorResult(string responseContent, out ErrorContract errorContract)
        {
            errorContract = null;
            if (!(responseContent.StartsWith("{") && responseContent.EndsWith("}")))
            {
                return false;
            }

            try
            {
                errorContract = responseContent.Deserialize<ErrorContract>();
                return true;
            }
            catch (JsonSerializationException)
            {
                return false;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}