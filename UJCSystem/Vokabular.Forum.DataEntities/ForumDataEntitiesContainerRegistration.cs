using DryIoc;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.Container;

namespace Vokabular.ForumSite.DataEntities
{
    public class ForumDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<BoardRepository>();
            container.AddPerWebRequest<CategoryRepository>();
            container.AddPerWebRequest<ForumRepository>();
            container.AddPerWebRequest<MessageRepository>();
            container.AddPerWebRequest<TopicRepository>();
            container.AddPerWebRequest<UserRepository>();
        }
    }
}
