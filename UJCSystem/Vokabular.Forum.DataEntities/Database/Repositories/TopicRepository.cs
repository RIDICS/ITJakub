using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class TopicRepository : ForumDbRepositoryBase
    {
        public TopicRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual Topic GetFirstTopicInForum(int forumId)
        {
            return GetSession().QueryOver<Topic>()
                .Where(x => x.Forum.ForumID == forumId)
                .OrderBy(x => x.Posted).Asc
                .Take(1).SingleOrDefault();
        }
    }
}

