using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumRepository : NHibernateDao
    {
        public ForumRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual Forum GetForumByName(string name)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }

        public virtual Forum GetForumByExternalCategoryIdAndCategory(int externalCategoryId, Category category)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.ExternalId == externalCategoryId)
                .Where(x => x.Category == category)
                .SingleOrDefault();
        }

        public virtual IList<Forum> GetForumsByExternalCategoryIds(ICollection categoryIds)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.ExternalId.IsIn(categoryIds))
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

