using System;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;

namespace Vokabular.FulltextService.Core.Managers
{
    public class TextResourceManager : ElasticsearchManagerBase
    {
        public TextResourceManager(CommunicationProvider communicationProvider) : base(communicationProvider)
        {}

        public TextResourceContract GetTextResource(string textResourceId)
        {
            var client = CommunicationProvider.GetElasticClient();
            var response = client.Get<TextResourceContract>(textResourceId, idx => idx.Index(Index).Type(PageType));
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            return response.Source;
        }

        public ResultContract CreateTextResource(TextResourceContract textResource)
        {
            var client = CommunicationProvider.GetElasticClient();
            var response = client.Index(textResource, idx => idx.Index(Index).Type(PageType));

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            if (response.Created)
            {
                return new ResultContract {Id = response.Id};
            }
            return new ResultContract {Id = null};
        }
    }
}