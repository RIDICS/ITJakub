using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Managers
{
    public class ImportedProjectMetadataManager
    {
        private readonly ImportedProjectMetadataRepository m_importedProjectMetadataRepository;

        public ImportedProjectMetadataManager(ImportedProjectMetadataRepository importedProjectMetadataRepository)
        {
            m_importedProjectMetadataRepository = importedProjectMetadataRepository;
        }

        public virtual ImportedProjectMetadata GetImportedProjectMetadataByExternalId(string externalId)
        {
            var result = m_importedProjectMetadataRepository.InvokeUnitOfWork(x => x.GetImportedProjectMetadata(externalId));
            return result;
        }
    }
}