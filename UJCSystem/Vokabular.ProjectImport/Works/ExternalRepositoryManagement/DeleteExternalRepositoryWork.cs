using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Works.ExternalRepositoryManagement
{
    public class DeleteExternalRepositoryWork : UnitOfWorkBase
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly int m_externalRepositoryId;

        public DeleteExternalRepositoryWork(ExternalRepositoryRepository externalRepositoryRepository, int externalRepositoryId) : base(externalRepositoryRepository)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_externalRepositoryId = externalRepositoryId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var externalRepository = m_externalRepositoryRepository.Load<ExternalRepository>(m_externalRepositoryId);

            if (externalRepository == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }
            
            if (externalRepository.ImportHistories == null || externalRepository.ImportHistories.Count > 0)
            {
                throw new MainServiceException(MainServiceErrorCode.RepositoryContainsHistory, "The external repository cannot be deleted, because it contains history.", HttpStatusCode.BadRequest);
            }

            m_externalRepositoryRepository.Delete(externalRepository);
        }
    }
}