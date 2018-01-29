using System;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers.Validators;
using Vokabular.FulltextService.Core.Options;
using Vokabular.FulltextService.DataContracts.Contracts;

namespace Vokabular.FulltextService.Core.Managers
{
    public class TextResourceManager : ElasticsearchManagerBase
    {
        private readonly ITextValidator m_textValidator;
        public TextResourceManager(CommunicationProvider communicationProvider, IOptions<IndicesOption> indicesOptions, ITextValidator textValidator)
            : base(communicationProvider, indicesOptions)
        {
            m_textValidator = textValidator;
        }

        public TextResourceContract GetTextResource(string textResourceId)
        {
            var client = CommunicationProvider.GetElasticClient();
            var response = client.Get<TextResourceContract>(textResourceId, idx => idx.Index(PageIndex).Type(PageType));
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            return response.Source;
        }

        public ResultContract CreateTextResource(TextResourceContract textResource)
        {
            var validationResult = m_textValidator.Validate(textResource.PageText);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var client = CommunicationProvider.GetElasticClient();
            var response = client.Index(textResource, idx => idx.Index(PageIndex).Type(PageType));

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            return new ResultContract {Id = response.Id};
        }
    }
}