using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Works.FilteringExpressionSetManagement
{
    public class DeleteFilteringExpressionSetWork : UnitOfWorkBase
    {
        private readonly FilteringExpressionSetRepository m_filteringExpressionSetRepository;
        private readonly int m_filteringExpressionSetId;

        public DeleteFilteringExpressionSetWork(FilteringExpressionSetRepository filteringExpressionSetRepository, int filteringExpressionSetId) : base(filteringExpressionSetRepository)
        {
            m_filteringExpressionSetRepository = filteringExpressionSetRepository;
            m_filteringExpressionSetId = filteringExpressionSetId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var filteringExpressionSet = m_filteringExpressionSetRepository.Load<FilteringExpressionSet>(m_filteringExpressionSetId);

            if (filteringExpressionSet == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            m_filteringExpressionSetRepository.Delete(filteringExpressionSet);
        }
    }
}