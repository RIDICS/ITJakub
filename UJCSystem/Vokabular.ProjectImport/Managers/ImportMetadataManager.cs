using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectImport.Works;

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

       /* public ImportMetadata GetImportMetadataByExternalId(string externalId)
        {
            var result = new CreateImportedMetadataWork(projectRepository, projectImportMetadata, userId).Execute();
            return result;
        }*/
        
    }
}
