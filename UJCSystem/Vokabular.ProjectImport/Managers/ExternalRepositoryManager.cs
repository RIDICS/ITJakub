using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.ProjectImport.Managers
{
    public class ExternalRepositoryManager
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;

        public ExternalRepositoryManager(ExternalRepositoryRepository externalRepositoryRepository)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
        }

        public ExternalRepository GetExternalRepository(int externalRepositoryId)
        {
            return m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetExternalRepository(externalRepositoryId));
        }
    }
}
