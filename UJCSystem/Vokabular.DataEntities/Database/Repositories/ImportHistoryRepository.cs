using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportHistoryRepository : NHibernateDao
    {
        public ImportHistoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}