using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.RestClient;
using Vokabular.Shared;

namespace Vokabular.FulltextService.DataContracts.Clients
{
    public class FulltextServiceClient : FullRestClientBase
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<FulltextServiceClient>();

        public FulltextServiceClient(Uri baseAddress) : base(baseAddress)
        {
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }
    }
}
