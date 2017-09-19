using System;
using System.Collections.Generic;
using System.Text;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class CategoryCriteriaImplementation : ICriteriaImplementationBase
    {
        private readonly CategoryRepository m_categoryRepository;

        public CategoryCriteriaImplementation(CategoryRepository categoryRepository)
        {
            m_categoryRepository = categoryRepository;
        }

        public CriteriaKey CriteriaKey => CriteriaKey.SelectedCategory;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var selectedCategoryContract = (SelectedCategoryCriteriaContract)searchCriteriaContract;
            var projectIds = selectedCategoryContract.SelectedBookIds == null || selectedCategoryContract.SelectedBookIds.Count == 0
                ? null
                : selectedCategoryContract.SelectedBookIds;
            var subcategoryIds = selectedCategoryContract.SelectedCategoryIds == null || selectedCategoryContract.SelectedCategoryIds.Count == 0
                ? null
                : m_categoryRepository.InvokeUnitOfWork(x => x.GetAllSubcategoryIds(selectedCategoryContract.SelectedCategoryIds));

            if (subcategoryIds == null || subcategoryIds.Count == 0)
                subcategoryIds = null;
            
            var whereBuilder = new StringBuilder();
            var joinBuilder = new StringBuilder();

            
            if (selectedCategoryContract.BookType != null)
            {
                var bookTypeParameterName = $"param{metadataParameters.Count}";

                joinBuilder.Append("inner join snapshot.BookTypes bookType ");
                whereBuilder.AppendFormat("bookType.Type = :{0}", bookTypeParameterName);
                metadataParameters.Add(bookTypeParameterName, selectedCategoryContract.BookType);
            }

            if (subcategoryIds != null || projectIds != null)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" and");

                whereBuilder.Append(" (");

                if (subcategoryIds != null)
                {
                    var categoryUniqueParameterName = $"param{metadataParameters.Count}";

                    joinBuilder.Append("left join project.Categories category ");
                    whereBuilder.AppendFormat("category.Id in (:{0})", categoryUniqueParameterName);
                    metadataParameters.Add(categoryUniqueParameterName, subcategoryIds);
                }

                if (subcategoryIds != null && projectIds != null)
                {
                    whereBuilder.Append(" or ");
                }

                if (projectIds != null)
                {
                    var bookUniqueParameterName = $"param{metadataParameters.Count}";

                    whereBuilder.AppendFormat("project.Id in (:{0})", bookUniqueParameterName);
                    metadataParameters.Add(bookUniqueParameterName, projectIds);
                }

                whereBuilder.Append(" )");
            }
            
            return new SearchCriteriaQuery
            {
                Join = joinBuilder.ToString(),
                Where = whereBuilder.ToString()
            };
        }
    }
}