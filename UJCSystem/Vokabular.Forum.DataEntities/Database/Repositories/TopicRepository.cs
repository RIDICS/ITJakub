using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class TopicRepository : NHibernateDao
    {
        public TopicRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

