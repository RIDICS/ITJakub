using System.Collections.Generic;
using NHibernate;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories.BibliographyImport
{
    public class ImportedRecordMetadataRepository : MainDbRepositoryBase
    {
        public ImportedRecordMetadataRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual ImportedRecordMetadata GetImportedRecordMetadataBySnapshot(long snapshotId)
        {
            return GetSession().QueryOver<ImportedRecordMetadata>()
                .Where(x=> x.Snapshot.Id == snapshotId)
                .Fetch(SelectMode.Fetch, x => x.LastUpdate)
                .Fetch(SelectMode.Fetch, x => x.ImportedProjectMetadata)
                .SingleOrDefault();
        }

        public virtual IList<ImportedRecordMetadata>  GetImportedRecordMetadataByImportedProjectMetadata(int importedProjectMetadataId)
        {
            return GetSession().QueryOver<ImportedRecordMetadata>()
                .Where(x=> x.ImportedProjectMetadata.Id == importedProjectMetadataId)
                .Fetch(SelectMode.Fetch, x => x.LastUpdate)
                .Fetch(SelectMode.Fetch, x => x.ImportedProjectMetadata)
                .List();
        }
    }
}