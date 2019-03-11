using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public class ImportMetadataManager
    {
        private readonly ImportMetadataRepository m_importMetadataRepository;

        public ImportMetadataManager(ImportMetadataRepository importMetadataRepository)
        {
            m_importMetadataRepository = importMetadataRepository;
        }

        public ImportMetadata GetImportMetadataByExternalId(string externalId)
        {
            var result = m_importMetadataRepository.InvokeUnitOfWork(x => x.GetExternalRepository(externalId));
            return result;
        }

        public int CreateImportMetadata(ProjectImportMetadata projectImportMetadata, ImportHistory importHistory)
        {
            var result = new CreateImportMetadataWork(m_importMetadataRepository, projectImportMetadata, importHistory).Execute();
            return result;
        }
    }
}