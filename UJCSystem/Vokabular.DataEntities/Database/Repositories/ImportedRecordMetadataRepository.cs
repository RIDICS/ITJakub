using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportedRecordMetadataRepository : NHibernateDao
    {
        public ImportedRecordMetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}