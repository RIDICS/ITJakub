using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        protected AreaController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public abstract BookTypeEnumContract AreaBookType { get; }

        protected BooksAndCategoriesContract GetBooksAndCategories()
        {
            using (var client = GetRestClient())
            {
                var categories = client.GetCategoryList();
                var books = client.GetBooksByType(AreaBookType);

                // Modify data for DropDownSelect usage

                const int rootCategoryId = 0;
                var rootBookTypeCategory = new CategoryContract
                {
                    Id = rootCategoryId,
                    Description = GetCategoryName(),
                    ParentCategoryId = null,
                };

                foreach (var category in categories)
                {
                    if (category.ParentCategoryId == null)
                        category.ParentCategoryId = rootCategoryId;
                }
                categories.Add(rootBookTypeCategory);


                var booksResult = new List<BookWithCategoryIdsContract>();
                foreach (var book in books)
                {
                    var categoryIds = book.CategoryList.Select(x => x.Id).ToList();
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

                var result = new BooksAndCategoriesContract
                {
                    BookType = AreaBookType,
                    Categories = categories,
                    Books = booksResult
                };
                return result;
            }
        }

        private string GetCategoryName()
        {
            switch (AreaBookType)
            {
                case BookTypeEnumContract.Edition:
                    return "Edice";
                case BookTypeEnumContract.Dictionary:
                    return "Slovníky";
                case BookTypeEnumContract.Grammar:
                    return "Digitalizované mluvnice";
                case BookTypeEnumContract.ProfessionalLiterature:
                    return "Odborná literatura";
                case BookTypeEnumContract.TextBank:
                    return "Textová banka";
                case BookTypeEnumContract.BibliographicalItem:
                    return "Bibliografie";
                case BookTypeEnumContract.CardFile:
                    return "Kartotéky";
                case BookTypeEnumContract.AudioBook:
                    return "Audio knihy";
                default:
                    return null;
            }
        }

        protected long SearchByCriteriaCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    BookType = AreaBookType,
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            using (var client = GetRestClient())
            {
                var request = new SearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                };
                var count = client.SearchBookCount(request);
                return count;
            }
        }

        protected List<SearchResultContract> SearchByCriteria(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            //listSearchCriteriaContracts.Add(new ResultCriteriaContract
            //{
            //    Start = start,
            //    Count = count,
            //    Sorting = (SortEnum)sortingEnum,
            //    Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
            //    //HitSettingsContract = new HitSettingsContract // TODO currently not used
            //    //{
            //    //    ContextLength = 50,
            //    //    Count = 3,
            //    //    Start = 1
            //    //}
            //});

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    BookType = AreaBookType,
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            using (var client = GetRestClient())
            {
                var request = new SearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                    Start = start,
                    Count = count,
                    Sort = (SortTypeEnumContract) sortingEnum,
                    SortDirection = sortAsc ? SortDirectionEnumContract.Asc : SortDirectionEnumContract.Desc,
                };
                var result = client.SearchBook(request);
                return result;
            }
        }
    }
}