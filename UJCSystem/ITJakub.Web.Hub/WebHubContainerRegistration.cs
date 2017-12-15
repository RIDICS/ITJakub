using ITJakub.Web.DataEntities;
using ITJakub.Web.Hub.AppStart.Installers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Core.Markdown;
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
            container.AddPerWebRequest<AuthenticationManager>();

            container.AddPerWebRequest<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();

            container.Install(new AutoMapperInstaller(), new NHibernateInstaller(), new WebDataEntitiesContainerRegistration());
        }
    }
}
