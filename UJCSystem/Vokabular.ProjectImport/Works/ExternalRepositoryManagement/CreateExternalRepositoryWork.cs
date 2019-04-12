using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.ProjectImport.Works.ExternalRepositoryManagement
{
    public class CreateExternalRepositoryWork : UnitOfWorkBase<int>
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly ExternalRepositoryDetailContract m_data;
        private readonly int m_userId;

        public CreateExternalRepositoryWork(ExternalRepositoryRepository externalRepositoryRepository, ExternalRepositoryDetailContract data, int userId) : base(externalRepositoryRepository)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_data = data;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var bibliographicFormat = m_externalRepositoryRepository.Load<BibliographicFormat>(m_data.BibliographicFormat.Id);
            var externalRepositoryType = m_externalRepositoryRepository.Load<ExternalRepositoryType>(m_data.ExternalRepositoryType.Id);
            var user = m_externalRepositoryRepository.Load<User>(m_userId);

            var externalRepository = new ExternalRepository
            {
                Name = m_data.Name,
                Description = m_data.Description,
                Url = m_data.Url,
                Configuration = m_data.Configuration,
                License = m_data.License,
                BibliographicFormat = bibliographicFormat,
                ExternalRepositoryType = externalRepositoryType,
                CreatedByUser = user,
                FilteringExpressionSets = new List<FilteringExpressionSet>()
            };

            foreach (var filteringExpressionSet in m_data.FilteringExpressionSets)
            {
                externalRepository.FilteringExpressionSets.Add(m_externalRepositoryRepository.Load<FilteringExpressionSet>(filteringExpressionSet.Id));
            }

            var resultId = (int) m_externalRepositoryRepository.Create(externalRepository);
            return resultId;
        }
    }
}