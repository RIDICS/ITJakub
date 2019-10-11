using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.Shared.DataEntities.UnitOfWork;

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
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            var bibliographicFormat = m_filteringExpressionSetRepository.Load<BibliographicFormat>(m_data.BibliographicFormat.Id);
            
            filteringExpressionSet.Name = m_data.Name;
            filteringExpressionSet.BibliographicFormat = bibliographicFormat;
            filteringExpressionSet.FilteringExpressions.Clear();
            var list = m_data.FilteringExpressions.Select(x => new FilteringExpression{Field = x.Field, Value = x.Value, FilteringExpressionSet = filteringExpressionSet}).ToList();
            foreach (var expression in list)
            {
                filteringExpressionSet.FilteringExpressions.Add(expression);
            }

            m_filteringExpressionSetRepository.Update(filteringExpressionSet);
        }
    }
}