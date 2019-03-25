using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.RestClient.Errors;

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
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            //TODO really delete?

            m_externalRepositoryRepository.Delete(externalRepository);
        }
    }
}