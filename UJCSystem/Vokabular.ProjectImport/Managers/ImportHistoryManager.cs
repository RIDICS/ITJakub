using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectImport.Works;

namespace Vokabular.ProjectImport.Managers
{
    public class ImportHistoryManager
    {
        private readonly ImportHistoryRepository m_importHistoryRepository;

        public ImportHistoryManager(ImportHistoryRepository importHistoryRepository)
        {
            m_importHistoryRepository = importHistoryRepository;
        }

        public int CreateImportHistory(int externalRepositoryId, int userId)
        {
            var resultId = new CreateImportHistoryWork(m_importHistoryRepository, externalRepositoryId, userId).Execute();
            return resultId;
        }

        public void UpdateImportHistory(ImportHistory importHistory)
        {
            m_importHistoryRepository.InvokeUnitOfWork(x => x.Update(importHistory));
        }

        public ImportHistory GetImportHistory(int importHistoryId)
        {
            return m_importHistoryRepository.InvokeUnitOfWork(x => x.FindById<ImportHistory>(importHistoryId));
        }

        public virtual ImportHistory GetLastImportHistoryForImportedProjectMetadata(int importedProjectMetadataId)
        {
            return m_importHistoryRepository.InvokeUnitOfWork(x => x.GetLastImportHistoryForImportedProjectMetadata(importedProjectMetadataId));
        }

        public ImportHistory GetLatestSuccessfulImportHistory(int externalRepositoryId)
        {
            return m_importHistoryRepository.InvokeUnitOfWork(x => x.GetLatestSuccessfulImportHistory(externalRepositoryId));
        }
        
        public ImportHistory GetLatestImportHistory(int externalRepositoryId)
        {
            return m_importHistoryRepository.InvokeUnitOfWork(x => x.GetLastImportHistory(externalRepositoryId));
        }
    }
}
