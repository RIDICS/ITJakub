using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class MessageRepository : NHibernateDao
    {
        public MessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

