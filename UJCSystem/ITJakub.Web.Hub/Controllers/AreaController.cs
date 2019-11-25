using System.Collections.Generic;
using System.Linq;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Helpers;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        private const int FetchBookCount = 200;

        private readonly CriteriaKey[] m_pageCriteriaKeys =
        {
            CriteriaKey.Fulltext,
            CriteriaKey.Heading,
            CriteriaKey.Sentence,
            CriteriaKey.Term,
            CriteriaKey.TokenDistance,
        };

        protected AreaController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        protected abstract BookTypeEnumContract AreaBookType { get; }

        public virtual ActionResult GetTypeaheadAuthor(string query)
        {
            var client = GetCodeListClient();
            var result = client.GetOriginalAuthorAutocomplete(query, AreaBookType);
            var resultStringList = result.Select(x => $"{x.LastName} {x.FirstName}");
            return Json(resultStringList);
        }

        public virtual ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var client = GetMetadataClient();
            var result = client.GetTitleAutocomplete(query, AreaBookType, GetDefaultProjectType(), selectedCategoryIds, selectedBookIds);
            return Json(result);
        }

        protected BooksAndCategoriesContract GetBooksAndCategories(bool fetchBooks = false)
        {
            var categoryClient = GetCodeListClient();
            var categories = categoryClient.GetCategoryList();
            
            // Modify data for DropDownSelect usage

            const int rootCategoryId = 0;
            var rootBookTypeCategory = new CategoryContract
            {
                Id = rootCategoryId,
                Description = BookTypeHelper.GetCategoryName(AreaBookType),
                ParentCategoryId = null,
            };

            foreach (var category in categories)
            {
                if (category.ParentCategoryId == null)
                    category.ParentCategoryId = rootCategoryId;
            }

            categories.Add(rootBookTypeCategory);

            var booksResult = new List<BookWithCategoryIdsContract>();

            if (fetchBooks)
            {
                var bookClient = GetBookClient();
                var books = bookClient.GetBooksByType(AreaBookType, 0, FetchBookCount);

                foreach (var book in books)
                {
                    var categoryIds = book.CategoryIds;
                    if (categoryIds.Count == 0)
                        categoryIds.Add(rootCategoryId);

                    booksResult.Add(new BookWithCategoryIdsContract
                    {
                        Id = book.Id,
                        Title = book.Title,
                        SubTitle = book.SubTitle,
                        Guid = null,
                        CategoryIds = categoryIds
                    });
                }
            }
            
            var result = new BooksAndCategoriesContract
            {
                BookType = AreaBookType,
                Categories = categories,
                Books = booksResult,
            };
            return result;
        }

        protected List<SearchCriteriaContract> CreateTextCriteriaList(CriteriaKey key, string text)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                var wordListCriteria = new WordListCriteriaContract
                {
                    Key = key,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                };
                listSearchCriteriaContracts.Add(wordListCriteria);
            }

            return listSearchCriteriaContracts;
        }

        protected void AddCategoryCriteria(List<SearchCriteriaContract> listSearchCriteriaContracts, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    BookType = AreaBookType,
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }
        }

        protected long SearchByCriteriaTextCount(CriteriaKey key, string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds, SearchAdvancedParametersContract parameters = null)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(key, text);

            return SearchByCriteriaCount(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds, parameters);
        }

        protected long SearchByCriteriaJsonCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds, SearchAdvancedParametersContract parameters = null)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            return SearchByCriteriaCount(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds, parameters);
        }

        protected long SearchByCriteriaCount(List<SearchCriteriaContract> listSearchCriteriaContracts, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds, SearchAdvancedParametersContract parameters)
        {
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var client = GetBookClient();
            var request = new AdvancedSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                Parameters = parameters,
            };
            var count = client.SearchBookCount(request, GetDefaultProjectType());
            return count;
        }

        protected List<SearchResultContract> SearchByCriteriaText(CriteriaKey key, string text, int start, int count, short sortingEnum,
            bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds, SearchAdvancedParametersContract parameters = null)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(key, text);

            return SearchByCriteria(listSearchCriteriaContracts, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds, parameters);
        }

        protected List<SearchResultContract> SearchByCriteriaJson(string json, int start, int count, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds, IList<int> selectedCategoryIds, SearchAdvancedParametersContract parameters = null)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            return SearchByCriteria(listSearchCriteriaContracts, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds, parameters);
        }

        protected List<SearchResultContract> SearchByCriteria(List<SearchCriteriaContract> listSearchCriteriaContracts, int start,
            int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds, SearchAdvancedParametersContract parameters)
        {
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var client = GetBookClient();
            var request = new AdvancedSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                Start = start,
                Count = count,
                Sort = (SortTypeEnumContract) sortingEnum,
                SortDirection = sortAsc ? SortDirectionEnumContract.Asc : SortDirectionEnumContract.Desc,
                FetchTerms = listSearchCriteriaContracts.Any(x => x.Key == CriteriaKey.Term),
                Parameters = parameters,
            };
            var result = client.SearchBook(request, GetDefaultProjectType());
            return result;
        }

        protected List<SearchCriteriaContract> GetOnlyPageCriteria(IList<SearchCriteriaContract> criteriaList)
        {
            var resultList = criteriaList.Where(x => m_pageCriteriaKeys.Contains(x.Key)).ToList();
            return resultList;
        }
    }
}