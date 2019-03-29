using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportedProjectMetadataRepository : NHibernateDao
    {
        public ImportedProjectMetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual ImportedProjectMetadata GetImportedProjectMetadata(string externalId)
        {
            return GetSession().QueryOver<ImportedProjectMetadata>()
                .Where(x => x.ExternalId == externalId)
                .Fetch(x => x.Project).Eager
                .SingleOrDefault();
        }
    }
}