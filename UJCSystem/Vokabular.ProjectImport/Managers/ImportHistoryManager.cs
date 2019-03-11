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

        public int CreateImportHistory(ExternalRepository externalRepository, int userId)
        {
            var resultId = new CreateImportHistoryWork(m_importHistoryRepository, externalRepository, userId).Execute();
            return resultId;
        }

        public ImportHistory GetImportHistory(int importHistoryId)
        {
            return m_importHistoryRepository.InvokeUnitOfWork(x => x.FindById<ImportHistory>(importHistoryId));
        }
    }
}
