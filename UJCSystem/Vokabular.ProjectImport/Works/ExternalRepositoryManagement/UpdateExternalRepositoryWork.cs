using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.ProjectImport.Works.ExternalRepositoryManagement
{
    public class UpdateExternalRepositoryWork : UnitOfWorkBase
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly int m_externalRepositoryId;
        private readonly ExternalRepositoryDetailContract m_data;

        public UpdateExternalRepositoryWork(ExternalRepositoryRepository externalRepositoryRepository, int externalRepositoryId, ExternalRepositoryDetailContract data) : base(externalRepositoryRepository)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_externalRepositoryId = externalRepositoryId;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var externalRepository = m_externalRepositoryRepository.FindById<ExternalRepository>(m_externalRepositoryId);
            if (externalRepository == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            var bibliographicFormat = m_externalRepositoryRepository.Load<BibliographicFormat>(m_data.BibliographicFormat.Id);
            var externalRepositoryType = m_externalRepositoryRepository.Load<ExternalRepositoryType>(m_data.ExternalRepositoryType.Id);

            externalRepository.Name = m_data.Name;
            externalRepository.Description = m_data.Description;
            externalRepository.Url = m_data.Url;
            externalRepository.Configuration = m_data.Configuration;
            externalRepository.License = m_data.License;
            externalRepository.BibliographicFormat = bibliographicFormat;
            externalRepository.ExternalRepositoryType = externalRepositoryType;

            externalRepository.FilteringExpressionSets.Clear();

            foreach (var filteringExpressionSet in m_data.FilteringExpressionSets)
            {
                externalRepository.FilteringExpressionSets.Add(m_externalRepositoryRepository.Load<FilteringExpressionSet>(filteringExpressionSet.Id));
            }

            m_externalRepositoryRepository.Update(externalRepository);
        }
    }
}