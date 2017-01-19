using ITJakub.Web.DataEntities;
using ITJakub.Web.Hub.Installers;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Managers.Markdown;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub
{
    public class WebHubContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<CommunicationProvider>();
            container.AddPerWebRequest<CommunicationConfigurationProvider>();
            container.AddPerWebRequest<StaticTextManager>();
            container.AddPerWebRequest<FeedbacksManager>();

            container.AddPerWebRequest<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();

            container.Install(new AutoMapperInstaller(), new NHibernateInstaller(), new WebDataEntitiesContainerRegistration());
        }
    }
}
