using NHibernate;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportHistoryRepository : MainDbRepositoryBase
    {
        public ImportHistoryRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public ImportHistory GetLatestSuccessfulImportHistory(int externalRepositoryId)
        {
            return GetSession().QueryOver<ImportHistory>()
                .Where(x => x.ExternalRepository.Id == externalRepositoryId)
                .AndRestrictionOn(x => x.Status).IsInG(new []{ImportStatusEnum.Completed, ImportStatusEnum.CompletedWithWarnings})
                .OrderBy(x => x.Date).Desc
                .Take(1)
                .SingleOrDefault();
        }

        public ImportHistory GetLastImportHistory(int externalRepositoryId)
        {
            return GetSession().QueryOver<ImportHistory>()
                .Where(x => x.ExternalRepository.Id == externalRepositoryId)
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .OrderBy(x => x.Date).Desc
                .Take(1)
                .SingleOrDefault();
        }

        public ImportHistory GetLastImportHistoryForImportedProjectMetadata(int importedProjectMetadataId)
        {
            ImportedRecordMetadata importedRecordMetadata = null;

            return  GetSession().QueryOver<ImportHistory>()
                .JoinAlias(x => x.ImportedRecordMetadata, () => importedRecordMetadata)
                .Where(() => importedRecordMetadata.ImportedProjectMetadata.Id == importedProjectMetadataId)
                .OrderBy(x => x.Date).Desc
                .Take(1)
                .SingleOrDefault();           
        }
    }
}