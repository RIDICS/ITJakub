using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumRepository : NHibernateDao
    {
        public ForumRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

