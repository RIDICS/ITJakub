using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportedRecordMetadataRepository : NHibernateDao
    {
        public ImportedRecordMetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual ImportedRecordMetadata GetImportedRecordMetadataBySnapshot(long snapshotId)
        {
            return GetSession().QueryOver<ImportedRecordMetadata>()
                .Where(x=> x.Snapshot.Id == snapshotId)
                .Fetch(x => x.LastUpdate).Eager
                .Fetch(x => x.ImportedProjectMetadata).Eager
                .SingleOrDefault();
        }

        public virtual IList<ImportedRecordMetadata>  GetImportedRecordMetadataByImportedProjectMetadata(int importedProjectMetadataId)
        {
            return GetSession().QueryOver<ImportedRecordMetadata>()
                .Where(x=> x.ImportedProjectMetadata.Id == importedProjectMetadataId)
                .Fetch(x => x.LastUpdate).Eager
                .Fetch(x => x.ImportedProjectMetadata).Eager
                .List();
        }
    }
}