using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
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

        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.SelectedCategory; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            return new SearchCriteriaQuery
            {
                Join = string.Empty,
                Where = string.Empty
            };
            //var selectedCategoryContract = (SelectedCategoryCriteriaContract) searchCriteriaContract;
            //var subcategoryIds = m_categoryRepository.GetAllSubcategoryIds(selectedCategoryContract.SelectedCategoryIds);
            
            //var categoryAlias = string.Format("c{0}", Guid.NewGuid().ToString("N"));
            //var bookAlias = string.Format("b{0}", Guid.NewGuid().ToString("N"));
            //var whereBuilder = new StringBuilder();

            //var bookUniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
            //var categoryUniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));

            //whereBuilder.Append(" (");

            //if (selectedCategoryContract.SelectedBookIds != null && selectedCategoryContract.SelectedBookIds.Count > 0)
            //{
            //    whereBuilder.Append(string.Format("{0}.Id in (:{1})", bookAlias, bookUniqueParameterName));
            //    metadataParameters.Add(bookUniqueParameterName, selectedCategoryContract.SelectedBookIds);
            //}

            //if ((selectedCategoryContract.SelectedBookIds != null && selectedCategoryContract.SelectedBookIds.Count > 0) &&
            //    (subcategoryIds != null && subcategoryIds.Count > 0))
            //{
            //    whereBuilder.Append(" or ");
            //}

            //if (subcategoryIds != null && subcategoryIds.Count > 0)
            //{
            //    whereBuilder.Append(string.Format("{0}.Id in (:{1})", categoryAlias, categoryUniqueParameterName));
            //    metadataParameters.Add(categoryUniqueParameterName, subcategoryIds);
            //}

            //whereBuilder.Append(" )");
            
            //return new SearchCriteriaQuery
            //{
            //    Join = string.Format("inner join bv.Categories {0} inner join bv.Book {1}", categoryAlias, bookAlias),
            //    Where = whereBuilder.ToString()
            //};
        }
    }
}