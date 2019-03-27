using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.ProjectImport.Works.FilteringExpressionSetManagement
{
    public class UpdateFilteringExpressionSetWork : UnitOfWorkBase
    {
        private readonly int m_filteringExpressionSetId;
        private readonly FilteringExpressionSetDetailContract m_data;
        private readonly FilteringExpressionSetRepository m_filteringExpressionSetRepository;

        public UpdateFilteringExpressionSetWork(FilteringExpressionSetRepository filteringExpressionSetRepository, FilteringExpressionSetDetailContract data, int filteringExpressionSetId) : base(filteringExpressionSetRepository)
        {
            m_filteringExpressionSetRepository = filteringExpressionSetRepository;
            m_data = data;
            m_filteringExpressionSetId = filteringExpressionSetId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var filteringExpressionSet = m_filteringExpressionSetRepository.FindById<FilteringExpressionSet>(m_filteringExpressionSetId);
            if (filteringExpressionSet == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            var bibliographicFormat = m_filteringExpressionSetRepository.Load<BibliographicFormat>(m_data.BibliographicFormat.Id);
            
            filteringExpressionSet.Name = m_data.Name;
            filteringExpressionSet.BibliographicFormat = bibliographicFormat;

            //TODO update FilteringExpressions

            m_filteringExpressionSetRepository.Update(filteringExpressionSet);
        }
    }
}