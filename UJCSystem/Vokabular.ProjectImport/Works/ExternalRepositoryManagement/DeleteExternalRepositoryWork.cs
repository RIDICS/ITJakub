using System;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
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
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            if (externalRepository.ImportHistories == null || externalRepository.ImportHistories.Count > 0)
            {
                throw new InvalidOperationException($"External repository {externalRepository.Name} cannot be deleted. The external repository contains history.");
            }

            m_externalRepositoryRepository.Delete(externalRepository);
        }
    }
}