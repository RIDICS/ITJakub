using System;
using System.Collections.Generic;
using System.Text;
using ITJakub.DataEntities.Database;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.ITJakubService.Core.Search
{
    public interface ICriteriaImplementationBase
    {
        CriteriaKey CriteriaKey { get; }
        SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters);
    }

    public class AuthorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Author; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var authorAlias = string.Format("a{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();
            

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" {0}.Name like (:{1})", authorAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Authors {0}", authorAlias),
                Where = whereBuilder.ToString()
            };
        }
    }

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
            var selectedCategoryContract = (SelectedCategoryCriteriaContract) searchCriteriaContract;
            var subcategoryIds = m_categoryRepository.GetAllSubcategoryIds(selectedCategoryContract.SelectedCategoryIds);
            
            var categoryAlias = string.Format("c{0}", Guid.NewGuid().ToString("N"));
            var bookAlias = string.Format("b{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            var bookUniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
            var categoryUniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));

            whereBuilder.Append(" (");

            if (selectedCategoryContract.SelectedBookIds != null && selectedCategoryContract.SelectedBookIds.Count > 0)
            {
                whereBuilder.Append(string.Format("{0}.Id in (:{1})", bookAlias, bookUniqueParameterName));
                metadataParameters.Add(bookUniqueParameterName, selectedCategoryContract.SelectedBookIds);
            }

            if ((selectedCategoryContract.SelectedBookIds != null && selectedCategoryContract.SelectedBookIds.Count > 0) &&
                (subcategoryIds != null && subcategoryIds.Count > 0))
            {
                whereBuilder.Append(" or ");
            }

            if (subcategoryIds != null && subcategoryIds.Count > 0)
            {
                whereBuilder.Append(string.Format("{0}.Id in (:{1})", categoryAlias, categoryUniqueParameterName));
                metadataParameters.Add(categoryUniqueParameterName, subcategoryIds);
            }

            whereBuilder.Append(" )");
            
            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Categories {0} inner join bv.Book {1}", categoryAlias, bookAlias),
                Where = whereBuilder.ToString()
            };
        }
    }

    public class TitleCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Title; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" bv.Title like (:{0})", uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Empty,
                Where = whereBuilder.ToString(),
            };
        }
    }

    public class EditorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Editor; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var responsiblesAlias = string.Format("r{0}", Guid.NewGuid().ToString("N"));
            var responsibleTypeAlias = string.Format("t{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");
                
                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" {0}.Text like (:{1})", responsiblesAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Responsibles {0} inner join {0}.ResponsibleType {1}", responsiblesAlias, responsibleTypeAlias),
                Where = string.Format("{0}.Text like 'Editor' and ({1})", responsibleTypeAlias, whereBuilder),
            };
        }
    }

    public class DatingCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Dating; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var datingListCriteriaContract = (DatingListCriteriaContract) searchCriteriaContract;
            var manuscriptAlias = string.Format("m{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            foreach (DatingCriteriaContract datingCriteriaContract in datingListCriteriaContract.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or ");

                var notBeforeUsed = false;
                whereBuilder.Append("(");

                if (datingCriteriaContract.NotBefore != null)
                {
                    notBeforeUsed = true;

                    var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                    whereBuilder.AppendFormat("{0}.NotAfter >= (:{1})", manuscriptAlias, uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, datingCriteriaContract.NotBefore.Value);
                }

                if (datingCriteriaContract.NotAfter != null)
                {
                    if (notBeforeUsed)
                    {
                        whereBuilder.Append(" and ");
                    }

                    var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                    whereBuilder.AppendFormat("{0}.NotBefore <= (:{1})", manuscriptAlias,uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, datingCriteriaContract.NotAfter.Value);
                }
                whereBuilder.Append(")");
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.ManuscriptDescriptions {0}", manuscriptAlias),
                Where = whereBuilder.ToString(),
            };
        }
    }

    public class HeadwordCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Headword; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var bookHeadwordAlias = string.Format("bh{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" or");
                }

                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" {0}.Headword like (:{1})", bookHeadwordAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.BookHeadwords {0}", bookHeadwordAlias),
                Where = whereBuilder.ToString()
            };
        }
    }

    public class TermCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Term; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract)searchCriteriaContract;
            var pageAlias = string.Format("pa{0}", Guid.NewGuid().ToString("N"));
            var termAlias = string.Format("ta{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" {0}.Text like (:{1})", termAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.BookPages {0} inner join {0}.Terms {1}", pageAlias, termAlias),
                Where = whereBuilder.ToString(),
            };
        }
    }

    public class AuthorizationCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Authorization; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var authorizationCriteria = (AuthorizationCriteriaContract)searchCriteriaContract;

            var bookAlias = string.Format("ba{0}", Guid.NewGuid().ToString("N"));
            var permissionAlias = string.Format("pa{0}", Guid.NewGuid().ToString("N"));
            var groupAlias = string.Format("ga{0}", Guid.NewGuid().ToString("N"));
            var userAlias = string.Format("ua{0}", Guid.NewGuid().ToString("N"));

            var userUniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
            metadataParameters.Add(userUniqueParameterName, authorizationCriteria.UserId);
          
            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Book {0} inner join {0}.Permissions {1} inner join {1}.Group {2} inner join {2}.Users {3}", bookAlias, permissionAlias, groupAlias, userAlias),
                Where = string.Format("{0}.Id = (:{1})", userAlias, userUniqueParameterName),
            };
        }
    }
}