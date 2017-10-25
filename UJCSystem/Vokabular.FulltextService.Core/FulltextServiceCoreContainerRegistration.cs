using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.Core.Managers.Markdown;
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

            container.AddPerWebRequest<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();
        }
    }
}
