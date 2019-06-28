using System.Collections.Generic;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumRepository : ForumDbRepositoryBase
    {
        public ForumRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual Forum GetForumByName(string name)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }

        public virtual Forum GetForumByExternalCategoryIdAndCategory(int externalCategoryId, int categoryId)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.ExternalId == externalCategoryId)
                .Where(x => x.Category.CategoryID == categoryId)
                .SingleOrDefault();
        }

        public virtual IList<Forum> GetForumsByExternalCategoryIds(IEnumerable<int> categoryIds)
        {
            return GetSession().QueryOver<Forum>()
                .WhereRestrictionOn(x => x.ExternalId).IsInG(categoryIds)
                .List();
        }

        public virtual Forum GetMainForumByExternalProjectId(long externalProjectId)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.ExternalProjectId == externalProjectId)
                .Where(x => x.RemoteURL == null)
                .SingleOrDefault();
        }

        public virtual IList<Forum> GetForumsByExternalProjectId(long externalProjectId)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.ExternalProjectId == externalProjectId)
                .List();
        }
    }
}

