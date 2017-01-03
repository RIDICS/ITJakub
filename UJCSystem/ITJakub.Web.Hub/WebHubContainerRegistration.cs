using ITJakub.Web.Hub.Managers;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub
{
    public class WebHubContainerRegistration : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            container.AddPerWebRequest<CommunicationProvider>();
            container.AddPerWebRequest<CommunicationConfigurationProvider>();
            container.AddPerWebRequest<StaticTextManager>();
            container.AddPerWebRequest<FeedbacksManager>();
        }
    }
}
