using System.Collections.Generic;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumAccessRepository : ForumDbRepositoryBase
    {
        public ForumAccessRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual IList<ForumAccess> GetAllAccessesForForum(int forumId)
        {
            return GetSession().QueryOver<ForumAccess>()
                .Where(x => x.Forum.ForumID == forumId)
                .List();
        }
    }
}