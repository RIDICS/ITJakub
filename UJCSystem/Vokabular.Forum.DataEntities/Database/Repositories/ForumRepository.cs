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

        public virtual Forum GetForumByExternalIdAndCategory(int externalId, Category category)
        {
            return GetSession().QueryOver<Forum>()
                .Where(x => x.ExternalId == externalId)
                .Where(x => x.Category == category)
                .SingleOrDefault();
        }
    }
}

