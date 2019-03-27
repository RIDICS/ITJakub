using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.RestClient.Errors;

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
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            //TODO delete?
            //TODO what with connestinons to filtexpress
            //TODO what with conn to extRepo

            m_filteringExpressionSetRepository.Delete(filteringExpressionSet);
        }
    }
}