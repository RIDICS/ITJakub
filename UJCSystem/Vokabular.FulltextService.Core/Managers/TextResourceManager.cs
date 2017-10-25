using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;

namespace Vokabular.FulltextService.Core.Managers
{
    public class TextResourceManager
    {
        private const string Index = "module"; //TODO rename index and type
        private const string Type = "page";
        private readonly CommunicationProvider m_communicationProvider;

        public TextResourceManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public TextResourceContract GetTextResource(string textResourceId)
        {
            var client = m_communicationProvider.GetElasticClient();
            var response = client.Get<TextResourceContract>(textResourceId, idx => idx.Index(Index).Type(Type));
            return response.Source;
        }

        public ResultContract CreateTextResource(TextResourceContract textResource)
        {
            var client = m_communicationProvider.GetElasticClient();
            var response = client.Index(textResource, idx => idx.Index(Index).Type(Type));
            if (response.Created)
            {
                return new ResultContract {Id = response.Id};
            }
            return new ResultContract {Id = null};
        }
    }
}