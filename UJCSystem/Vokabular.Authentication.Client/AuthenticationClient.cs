using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.RestClient;
using Vokabular.Shared;

namespace Vokabular.Authentication.Client
{
    public class AuthenticationClient : FullRestClientBase
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<AuthenticationClient>();

        public AuthenticationClient(Uri baseAddress, string username, string password) : base(baseAddress, true)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }
    }

}
