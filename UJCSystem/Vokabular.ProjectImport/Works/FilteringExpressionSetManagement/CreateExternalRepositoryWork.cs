using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.ProjectImport.Works.FilteringExpressionSetManagement
{
    public class CreateFilteringExpressionSetWork : UnitOfWorkBase<int>
    {
        private readonly FilteringExpressionSetRepository m_filteringExpressionSetRepository;
        private readonly FilteringExpressionSetDetailContract m_data;
        private readonly int m_userId;

        public CreateFilteringExpressionSetWork(FilteringExpressionSetRepository filteringExpressionSetRepository, FilteringExpressionSetDetailContract data, int userId) : base(filteringExpressionSetRepository)
        {
            m_filteringExpressionSetRepository = filteringExpressionSetRepository;
            m_data = data;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var bibliographicFormat = m_filteringExpressionSetRepository.Load<BibliographicFormat>(m_data.BibliographicFormat.Id);
            var user = m_filteringExpressionSetRepository.Load<User>(m_userId);

            //TODO create FiltrExpression
            
            var externalRepository = new FilteringExpressionSet
            {
                Name = m_data.Name,
                CreatedByUser = user,
                BibliographicFormat = bibliographicFormat
            };
            var resultId = (int)m_filteringExpressionSetRepository.Create(externalRepository);
            return resultId;
        }
    }
}