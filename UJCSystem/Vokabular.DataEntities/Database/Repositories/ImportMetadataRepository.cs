using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportMetadataRepository : NHibernateDao
    {
        public ImportMetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual ImportMetadata GetImportMetadata(string externalId)
        {
            return GetSession().QueryOver<ImportMetadata>()
                .Where(x => x.ExternalId == externalId)
                .Fetch(x => x.Snapshot).Eager
                .SingleOrDefault();
        }
    }
}