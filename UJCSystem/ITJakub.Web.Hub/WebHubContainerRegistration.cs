using ITJakub.Web.Hub.Installers;
using ITJakub.Web.Hub.Managers;
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

            new AutoMapperInstaller().Install(container);
            new RepositoryInstaller().Install(container);
        }
    }
}
