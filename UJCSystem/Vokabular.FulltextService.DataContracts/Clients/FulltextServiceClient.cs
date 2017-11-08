using System;
using System.Collections.Generic;
using System.Net.Http;
using ITJakub.Shared.Contracts;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.DataContracts.Clients
{
    public class FulltextServiceClient : FullRestClientBase
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<FulltextServiceClient>();

        public FulltextServiceClient(Uri baseAddress) : base(baseAddress)
        {
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        public TextResourceContract GetTextResource(string resourceId, TextFormatEnumContract formatValue)
        {
            try
            {
                var textResource = Get<TextResourceContract>($"text/{resourceId}?formatValue={formatValue}");
                return textResource;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }


        public string CreateTextResource(string text, int versionNumber)
        {
            var textResource = new TextResourceContract{ Text = text, VersionNumber  = versionNumber};

            try
            {
                var result = Post<ResultContract>($"text", textResource);
                return result.Id;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void CreateSnapshot(long projectId, List<string> pageIds)
        {
            var snapshotResource = new SnapshotPageIdsResourceContract { PageIds = pageIds, ProjectId = projectId};

            try
            {
                var result = Post<ResultContract>($"snapshot", snapshotResource);
                
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
    }
    
}
