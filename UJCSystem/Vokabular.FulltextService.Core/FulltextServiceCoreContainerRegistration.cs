using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Helpers.Converters;
using Vokabular.FulltextService.Core.Helpers.Hml;
using Vokabular.FulltextService.Core.Helpers.Markdown;
using Vokabular.FulltextService.Core.Helpers.Validators;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.Shared.Container;

namespace Vokabular.FulltextService.Core
{
    public class FulltextServiceCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<CommunicationConfigurationProvider>();
            container.AddPerWebRequest<CommunicationProvider>();
            container.AddPerWebRequest<TextResourceManager>();
            container.AddPerWebRequest<SnapshotResourceManager>();
            container.AddPerWebRequest<SearchManager>();
            container.AddPerWebRequest<SearchResultProcessor>();
            container.AddPerWebRequest<QueriesBuilder>();
            container.AddPerWebRequest<SnapshotResourceBuilder>();

            container.AddPerWebRequest<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();
            container.AddPerWebRequest<IHtmlToPlainTextConverter, HtmlToPlainTextConverter>();
            container.AddPerWebRequest<ITextConverter, TextConverter>();
            container.AddPerWebRequest<ITextValidator, TextValidator>();
        }
    }
}
