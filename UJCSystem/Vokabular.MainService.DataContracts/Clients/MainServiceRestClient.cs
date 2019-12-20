﻿using System.Net;
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

            HttpClient.DefaultRequestHeaders.Add(MainServiceHeaders.PortalTypeHeader, configuration.PortalType.ToString());
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, m_tokenProvider.AuthToken);
        }

        protected override void TryParseAndThrowResponseError(HttpStatusCode responseStatusCode, string responseContent)
        {
            var statusCodeNumber = (int) responseStatusCode;
            if (statusCodeNumber >= 400 &&
                statusCodeNumber < 500 &&
                TryDeserialize<ErrorContract>(responseContent, out var errorContract))
            {
                var exception = new MainServiceException(errorContract.Code, errorContract.Description, responseStatusCode, errorContract.DescriptionParams);

                m_localization.LocalizeApiException(exception);

                throw exception;
            }

            if (responseStatusCode == HttpStatusCode.Forbidden)
            {
                var exception = new MainServiceException(MainServiceErrorCode.Forbidden, "Insufficient permissions", responseStatusCode);

                m_localization.LocalizeApiException(exception);

                throw exception;
            }
        }
    }
}