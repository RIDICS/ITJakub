using Vokabular.ForumSite.Core.Managers;
using Vokabular.ForumSite.DataEntities;
using Vokabular.Shared.Container;

namespace Vokabular.ForumSite.Core
{
    public class ForumCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<ForumManager>();

            container.Install<ForumDataEntitiesContainerRegistration>();  // TODO move to Forum.Core registration
        }
    }
}
