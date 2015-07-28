using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Core.SearchService;
using ITJakub.DataEntities.Database;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.Core.Search;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.MobileApps.MobileContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;
using BookContract = ITJakub.MobileApps.MobileContracts.BookContract;

namespace ITJakub.ITJakubService.Core
{
    public class SearchManager
    {
        private const int PrefetchRecordCount = 5;
        private readonly BookRepository m_bookRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly MetadataSearchCriteriaDirector m_searchCriteriaDirector;
        private readonly SearchServiceClient m_searchServiceClient;

        public SearchManager(BookRepository bookRepository, BookVersionRepository bookVersionRepository,
            CategoryRepository categoryRepository, MetadataSearchCriteriaDirector searchCriteriaDirector,
            SearchServiceClient searchServiceClient)
        {
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
            m_categoryRepository = categoryRepository;
            m_searchCriteriaDirector = searchCriteriaDirector;
            m_searchServiceClient = searchServiceClient;
        }

        public List<SearchResultContract> Search(string term)
        {
            var bookVersionResults = m_bookRepository.SearchByTitle(term);
            return Mapper.Map<List<SearchResultContract>>(bookVersionResults);
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var books = m_bookRepository.FindBooksLastVersionsByBookType(type);
            var categories = m_categoryRepository.FindCategoriesByBookType(type).OrderBy(x => x.Description);

            return new BookTypeSearchResultContract
            {
                BookType = bookType,
                Books = Mapper.Map<IList<Shared.Contracts.BookContract>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        private FilteredCriterias FilterSearchCriterias(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            ResultCriteriaContract resultCriteria = null;
            var nonMetadataCriterias = new List<SearchCriteriaContract>();
            var metadataCriterias = new List<SearchCriteriaContract>();
            var conjunction = new List<SearchCriteriaQuery>();
            var metadataParameters = new Dictionary<string, object>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    var criteriaQuery = m_searchCriteriaDirector.ProcessCriteria(searchCriteriaContract,
                        metadataParameters);
                    conjunction.Add(criteriaQuery);
                    metadataCriterias.Add(searchCriteriaContract);
                }
                else
                {
                    nonMetadataCriterias.Add(searchCriteriaContract);

                    if (searchCriteriaContract.Key == CriteriaKey.Result)
                        resultCriteria = (ResultCriteriaContract) searchCriteriaContract;
                }
            }

            return new FilteredCriterias
            {
                ResultCriteria = resultCriteria,
                MetadataParameters = metadataParameters,
                NonMetadataCriterias = nonMetadataCriterias,
                MetadataCriterias = metadataCriterias,
                ConjunctionQuery = conjunction
            };
        }

        public IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var filteredCriterias = FilterSearchCriterias(searchCriterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;

            if (nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().FirstOrDefault() == null)
            {
                var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery, filteredCriterias.MetadataParameters));
                
                if (databaseSearchResult.Count == 0)
                {
                    return new List<SearchResultContract>();
                }

                var resultContract = new ResultRestrictionCriteriaContract
                {
                    ResultBooks = databaseSearchResult
                };
                nonMetadataCriterias.Add(resultContract);   
            }

            if (filteredCriterias.NonMetadataCriterias.All(
                    x => x.Key == CriteriaKey.ResultRestriction || x.Key == CriteriaKey.Result))
            {
                // Search only in SQL
                var resultRestriction = nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().First();
                var guidListRestriction = resultRestriction.ResultBooks.Select(x => x.Guid);
                var resultBookVersions = m_bookVersionRepository.GetBookVersionsByGuid(guidListRestriction);
                return Mapper.Map<IList<SearchResultContract>>(resultBookVersions);
            }

            // Fulltext search
            var searchResults = m_searchServiceClient.ListSearchEditionsResults(nonMetadataCriterias);

            var guidList = searchResults.SearchResults.Select(x => x.BookXmlId);
            var result = m_bookVersionRepository.GetBookVersionsByGuid(guidList);

            var resultDictionary = result.ToDictionary(x => x.Book.Guid);

            var searchResultFullContext = new List<SearchResultContract>();

            foreach (var searchResult in searchResults.SearchResults)
            {
                var localResult = Mapper.Map<SearchResultContract>(resultDictionary[searchResult.BookXmlId]);
                localResult.TotalHitCount = searchResult.TotalHitCount;
                localResult.Results = searchResult.Results;
                searchResultFullContext.Add(localResult);
            }

            return searchResultFullContext;
        }

        public List<SearchResultContract> GetBooksByBookType(BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.FindBooksLastVersionsByBookType(type);
            return Mapper.Map<List<SearchResultContract>>(bookVersions);
        }

        public List<BookContract> GetBooksByBookType(BookTypeContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.FindBooksLastVersionsByBookType(type);
            return Mapper.Map<List<BookContract>>(bookVersions);
        }

        public List<SearchResultContract> SearchBooksWithBookType(string term, BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.SearchByTitleAndBookType(term, type);
            return Mapper.Map<List<SearchResultContract>>(bookVersions);
        }

        public IList<BookContract> Search(BookTypeContract category, SearchDestinationContract searchBy, string query)
        {
            var type = Mapper.Map<BookTypeEnum>(category);
            IList<BookVersion> bookList = null;

            switch (searchBy)
            {
                case SearchDestinationContract.Author:
                    //TODO search by author
                    break;
                default:
                    bookList = m_bookRepository.SearchByTitleAndBookType(query, type);
                    break;
            }
            return Mapper.Map<IList<BookContract>>(bookList);
        }

        private string PrepareQuery(string query)
        {
            query = query.TrimStart().TrimEnd().Replace(" ", "% %");
            return string.Format("%{0}%", query);
        }

        public IList<string> GetTypeaheadAuthors(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastAuthors(PrefetchRecordCount);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadAuthors(query, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookTypeContract)
        {
            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastAuthorsByBookType(PrefetchRecordCount, bookType);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadAuthorsByBookType(query, bookType, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadTitles(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTitles(PrefetchRecordCount);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadTitles(query, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookTypeContract, IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var bookIdList = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTitlesByBookType(PrefetchRecordCount, bookType, bookIdList);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadTitlesByBookType(query, bookType, bookIdList, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            string query)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTypeaheadHeadwords(PrefetchRecordCount, bookIds);

            query = string.Format("{0}%", query);
            return m_bookRepository.GetTypeaheadHeadwords(query, PrefetchRecordCount, bookIds);
        }

        private HeadwordListContract ConvertHeadwordSearchToContract(IList<HeadwordSearchResult> databaseResult)
        {
            var dictionaryList = new Dictionary<string, DictionaryContract>();
            var headwordList = new List<HeadwordContract>();
            var headwordContract = new HeadwordContract();
            foreach (var headword in databaseResult)
            {
                DictionaryContract dictionaryContract;
                if (!dictionaryList.TryGetValue(headword.BookGuid, out dictionaryContract))
                {
                    dictionaryContract = new DictionaryContract
                    {
                        BookAcronym = headword.BookAcronym,
                        BookId = 0, // TODO
                        BookTitle = headword.BookTitle,
                        BookXmlId = headword.BookGuid,
                        BookVersionXmlId = null, //TODO
                        BookVersionId = 0 // TODO
                    };
                    dictionaryList.Add(dictionaryContract.BookXmlId, dictionaryContract);
                }

                var bookInfoContract = new HeadwordBookInfoContract
                {
                    BookXmlId = headword.BookGuid,
                    EntryXmlId = headword.XmlEntryId
                };

                if (headword.Headword == headwordContract.Headword)
                {
                    headwordContract.Dictionaries.Add(bookInfoContract);
                }
                else
                {
                    headwordContract = new HeadwordContract
                    {
                        Dictionaries = new List<HeadwordBookInfoContract> {bookInfoContract},
                        Headword = headword.Headword
                    };
                    headwordList.Add(headwordContract);
                }
            }

            return new HeadwordListContract
            {
                BookList = dictionaryList,
                HeadwordList = headwordList
            };
        }

        private IList<long> GetCompleteBookIdList(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            if (selectedCategoryIds != null && selectedCategoryIds.Count > 0)
            {
                var bookIdsFromCategory = m_categoryRepository.GetBookIdsFromCategory(selectedCategoryIds);

                return selectedBookIds == null
                    ? bookIdsFromCategory
                    : bookIdsFromCategory.Concat(selectedBookIds).ToList();
            }

            return selectedBookIds;
        }

        private string GetSingleHeadwordQuery(IList<SearchCriteriaContract> searchCriteria)
        {
            var headwordCriteria =
                searchCriteria.Where(x => x.Key == CriteriaKey.Headword)
                    .OfType<WordListCriteriaContract>()
                    .SingleOrDefault();

            if (headwordCriteria == null)
                return null;

            var wordCriteria = headwordCriteria.Disjunctions.SingleOrDefault();
            if (wordCriteria == null)
                return null;

            return CriteriaConditionBuilder.Create(wordCriteria);
        }

        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            return bookIds == null
                ? m_bookVersionRepository.GetHeadwordCount()
                : m_bookVersionRepository.GetHeadwordCount(bookIds);
        }

        public HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            int start, int end)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            var databaseResult = bookIds == null
                ? m_bookVersionRepository.GetHeadwordList(start, end)
                : m_bookVersionRepository.GetHeadwordList(start, end, bookIds);
            var result = ConvertHeadwordSearchToContract(databaseResult);

            return result;
        }

        public int GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            return m_bookVersionRepository.GetHeadwordRowNumber(bookIds, query);
        }

        public int GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            return m_bookVersionRepository.GetHeadwordRowNumberById(bookIds, headwordBookId, headwordEntryXmlId);
        }

        public int SearchHeadwordByCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias,
            DictionarySearchTarget searchTarget)
        {
            var filteredCriterias = FilterSearchCriterias(searchCriterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;
            var creator = new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery, filteredCriterias.MetadataParameters);
            
            // Only SQL search
            if (searchTarget == DictionarySearchTarget.Headword)
            {
                var query = GetSingleHeadwordQuery(filteredCriterias.MetadataCriterias);
                if (query == null)
                    return 0;

                creator.SetHeadwordQueryParameter(query);

                var resultCount = m_bookVersionRepository.GetSearchHeadwordCount(creator);
                return resultCount;
            }

            // Advanced search
            var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(creator);
            if (databaseSearchResult.Count == 0)
                return 0;

            if (nonMetadataCriterias.All(x => x.Key == CriteriaKey.Result))
            {
                // Search only in SQL
                var headwordQueryCreator = new HeadwordCriteriaQueryCreator();
                headwordQueryCreator.AddCriteria(filteredCriterias.MetadataCriterias);
                var bookGuidList = databaseSearchResult.Select(x => x.Guid);
                var resultCount = m_bookVersionRepository.GetHeadwordCountBySearchCriteria(bookGuidList, headwordQueryCreator);
                return resultCount;
            }

            // Fulltext search
            var resultContract = new ResultRestrictionCriteriaContract
            {
                ResultBooks = databaseSearchResult
            };
            var headwordContracts = filteredCriterias.MetadataCriterias.Where(x => x.Key == CriteriaKey.Headword);
            nonMetadataCriterias.Add(resultContract);
            nonMetadataCriterias.AddRange(headwordContracts);

            return m_searchServiceClient.ListSearchDictionariesResultsCount(nonMetadataCriterias);
        }

        public int SearchCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var filteredCriterias = FilterSearchCriterias(searchCriterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;
            var creator = new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery,
                filteredCriterias.MetadataParameters);
            var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(creator);
            if (databaseSearchResult.Count == 0)
                return 0;

            if (nonMetadataCriterias.All(x => x.Key == CriteriaKey.Result))
            {
                // Search only in SQL
                return databaseSearchResult.Count;
            }

            // Fulltext search
            var resultContract = new ResultRestrictionCriteriaContract
            {
                ResultBooks = databaseSearchResult
            };
            nonMetadataCriterias.Add(resultContract);

            return m_searchServiceClient.GetSearchCriteriaResultsCount(nonMetadataCriterias);
        }

        public HeadwordListContract SearchHeadwordByCriteria(
            IEnumerable<SearchCriteriaContract> searchCriterias, DictionarySearchTarget searchTarget)
        {
            var filteredCriterias = FilterSearchCriterias(searchCriterias);
            var resultCriteria = filteredCriterias.ResultCriteria;
            var creator = new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery, filteredCriterias.MetadataParameters);

            // Only SQL search
            if (searchTarget == DictionarySearchTarget.Headword)
            {
                if (resultCriteria.Start == null || resultCriteria.Count == null)
                    return null;

                var query = GetSingleHeadwordQuery(filteredCriterias.MetadataCriterias);
                if (query == null)
                    return new HeadwordListContract();

                creator.SetHeadwordQueryParameter(query);
                
                var databaseHeadwords = m_bookVersionRepository.SearchHeadwordByCriteria(creator, resultCriteria.Start.Value, resultCriteria.Count.Value);

                return ConvertHeadwordSearchToContract(databaseHeadwords);
            }

            // Advanced search
            var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(creator);
            if (databaseSearchResult.Count == 0)
                return new HeadwordListContract
                {
                    BookList = new Dictionary<string, DictionaryContract>(),
                    HeadwordList = new List<HeadwordContract>()
                };

            if (filteredCriterias.NonMetadataCriterias.All(x => x.Key == CriteriaKey.Result))
            {
                // Search only in SQL
                if (resultCriteria.Start == null || resultCriteria.Count == null)
                    return null;

                var headwordQueryCreator = new HeadwordCriteriaQueryCreator();
                headwordQueryCreator.AddCriteria(filteredCriterias.MetadataCriterias);
                var bookGuidList = databaseSearchResult.Select(x => x.Guid);
                var resultHeadwords = m_bookVersionRepository.GetHeadwordListBySearchCriteria(bookGuidList, headwordQueryCreator, resultCriteria.Start.Value, resultCriteria.Count.Value);
                return ConvertHeadwordSearchToContract(resultHeadwords);
            }

            // Fulltext search
            var resultRestrictionContract = new ResultRestrictionCriteriaContract
            {
                ResultBooks = databaseSearchResult
            };
            var headwordContracts = filteredCriterias.MetadataCriterias.Where(x => x.Key == CriteriaKey.Headword);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;
            nonMetadataCriterias.Add(resultRestrictionContract);
            nonMetadataCriterias.AddRange(headwordContracts);

            var resultContract = m_searchServiceClient.ListSearchDictionariesResults(nonMetadataCriterias);
            
            // fill book info
            var bookInfoList = m_bookVersionRepository.GetBookVersionsByGuid(resultContract.BookList.Keys);
            var bookInfoContracts = Mapper.Map<IList<DictionaryContract>>(bookInfoList);
            var bookDictionary = bookInfoContracts.ToDictionary(x => x.BookXmlId, x => x);
            resultContract.BookList = bookDictionary;

            return resultContract;
        }

        public string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid,
            string xmlEntryId, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat,
                bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;
            var dictionaryEntryText = m_searchServiceClient.GetDictionaryEntryFromSearch(searchCriterias.ToList(),
                bookGuid, bookVersion.VersionId, xmlEntryId, transformationName, resultFormat, transformationLevel);

            return dictionaryEntryText;
        }

        private class FilteredCriterias
        {
            public List<SearchCriteriaQuery> ConjunctionQuery { get; set; }
            public List<SearchCriteriaContract> NonMetadataCriterias { get; set; }
            public List<SearchCriteriaContract> MetadataCriterias { get; set; }
            public Dictionary<string, object> MetadataParameters { get; set; }
            public ResultCriteriaContract ResultCriteria { get; set; }
        }

        public PageListContract GetSearchEditionsPageList(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceClient.GetSearchEditionsPageList(searchCriterias.ToList());
        }

        public string GetEditionPageFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookXmlId,
            string pageXmlId, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var bookVersion = m_bookRepository.GetLastVersionForBook(bookXmlId);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat,
                bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            var pageText = m_searchServiceClient.GetEditionPageFromSearch(searchCriterias.ToList(),
                bookXmlId, bookVersion.VersionId, pageXmlId, transformationName, resultFormat, transformationLevel);

            return pageText;
        }
    }
}