using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumAccessRepository : NHibernateDao
    {
        public ForumAccessRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }
    }
}
