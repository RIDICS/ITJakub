using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class CategoryRepository : NHibernateDao
    {
        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual Category GetCategoryByName(string name)
        {
            return GetSession().QueryOver<Category>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }

        public virtual Category GetCategoryByExternalId(short externalId)
        {
            return GetSession().QueryOver<Category>()
                .Where(x => x.ExternalId == externalId)
                .SingleOrDefault();
        }
    }
}
