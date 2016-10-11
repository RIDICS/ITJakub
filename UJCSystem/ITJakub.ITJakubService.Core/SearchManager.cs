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
        private readonly AuthorizationManager m_authorizationManager;
        private readonly BookRepository m_bookRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly MetadataSearchCriteriaDirector m_searchCriteriaDirector;
        private readonly SearchServiceClient m_searchServiceClient;
        private readonly UserRepository m_userRepository;

        public SearchManager(BookRepository bookRepository, BookVersionRepository bookVersionRepository,
            CategoryRepository categoryRepository, UserRepository userRepository, PermissionRepository permissionRepository,
            MetadataSearchCriteriaDirector searchCriteriaDirector,
            AuthorizationManager authorizationManager, SearchServiceClient searchServiceClient)
        {
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
            m_categoryRepository = categoryRepository;
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_searchCriteriaDirector = searchCriteriaDirector;
            m_authorizationManager = authorizationManager;
            m_searchServiceClient = searchServiceClient;
        }

        public List<SearchResultContract> Search(string term)
        {
            var escapedTerm = EscapeQuery(term);
            var bookVersionResults = m_bookRepository.SearchByTitle(escapedTerm);
            m_authorizationManager.FilterBookVersionsByCurrentUser(ref bookVersionResults);
            return Mapper.Map<List<SearchResultContract>>(bookVersionResults);
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var books = m_bookRepository.FindBookVersionsByTypeWithCategories(type);
            m_authorizationManager.FilterBookVersionsByCurrentUser(ref books);
            var categories = m_categoryRepository.FindCategoriesByBookType(type);

            return new BookTypeSearchResultContract
            {
                BookType = bookType,
                Books = Mapper.Map<IList<BookContractWithCategories>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        public IList<CategoryContract> GetRootCategories()
        {
            var categories = m_categoryRepository.GetRootCategories();
            return Mapper.Map<IList<CategoryContract>>(categories);
        }

        private string EscapeQuery(string query)
        {
            return query.Replace("[", "[[]");
        }

        private FilteredCriterias FilterSearchCriterias(IList<SearchCriteriaContract> searchCriterias)
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
            var searchCriteriaContracts = searchCriterias.ToList();

            m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var filteredCriterias = FilterSearchCriterias(searchCriteriaContracts);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;
            var resultCriteria = filteredCriterias.ResultCriteria;

            var queryCreator = new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery, filteredCriterias.MetadataParameters);

            if (nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().FirstOrDefault() == null)
            {
                var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(queryCreator);

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

            var resultCriteriaContract = nonMetadataCriterias.OfType<ResultCriteriaContract>().FirstOrDefault();

            Dictionary<long, List<PageDescriptionContract>> bookTermResults = null;
            Dictionary<long, long> bookTermResultsCount = null;

            if (filteredCriterias.NonMetadataCriterias.All(x => x.Key == CriteriaKey.ResultRestriction || x.Key == CriteriaKey.Result))
            {
                // Search only in SQL
                var resultRestriction = nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().First();
                var guidListRestriction = resultRestriction.ResultBooks.Select(x => x.Guid).ToList();
                var resultBookVersions = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidListRestriction, resultCriteria.Start,
                    resultCriteria.Count, resultCriteria.Sorting, resultCriteria.Direction);
                var pageCounts = m_bookVersionRepository.GetBooksPageCountByGuid(guidListRestriction)
                    .ToDictionary(x => x.BookId, x => x.Count);


                if (resultCriteriaContract != null && resultCriteriaContract.TermsSettingsContract != null)
                {
                    var termQueryCreator = new TermCriteriaQueryCreator();
                    termQueryCreator.AddCriteria(filteredCriterias.MetadataCriterias);

                    var booksTermResults = m_bookVersionRepository.GetBooksTermResults(guidListRestriction, termQueryCreator);
                    bookTermResults =
                        booksTermResults.GroupBy(x => x.BookId, x => new PageDescriptionContract {PageName = x.PageName, PageXmlId = x.PageXmlId})
                            .ToDictionary(x => x.Key, x => x.ToList());

                    var booksTermResultsCount = m_bookVersionRepository.GetBooksTermResultsCount(guidListRestriction, termQueryCreator);
                    bookTermResultsCount = booksTermResultsCount.ToDictionary(x => x.BookId, x => x.PagesCount);
                }

                var resultContractList = Mapper.Map<IList<SearchResultContract>>(resultBookVersions);

                foreach (var resultContract in resultContractList)
                {
                    resultContract.PageCount = pageCounts[resultContract.BookId];

                    if (bookTermResults != null)
                    {
                        List<PageDescriptionContract> pageDescriptionContracts;
                        long termResultsCount;
                        resultContract.TermsPageHits = bookTermResults.TryGetValue(resultContract.BookId, out pageDescriptionContracts)
                            ? pageDescriptionContracts
                            : new List<PageDescriptionContract>();
                        resultContract.TermsPageHitsCount = bookTermResultsCount.TryGetValue(resultContract.BookId, out termResultsCount)
                            ? Convert.ToInt32(termResultsCount)
                            : 0;
                    }
                }

                return resultContractList;
            }

            // Fulltext search
            var searchResults = m_searchServiceClient.ListSearchEditionsResults(nonMetadataCriterias);

            var guidList = searchResults.SearchResults.Select(x => x.BookXmlId).ToList();
            var result = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidList);
            var resultPageCountDictionary = m_bookVersionRepository.GetBooksPageCountByGuid(guidList)
                .ToDictionary(x => x.BookId, x => x.Count);

            var resultDictionary = result.ToDictionary(x => x.Book.Guid);


            if (resultCriteriaContract != null && resultCriteriaContract.TermsSettingsContract != null)
            {
                var termQueryCreator = new TermCriteriaQueryCreator();
                termQueryCreator.AddCriteria(filteredCriterias.MetadataCriterias);

                var booksTermResults = m_bookVersionRepository.GetBooksTermResults(guidList, termQueryCreator);
                bookTermResults =
                    booksTermResults.GroupBy(x => x.BookId, x => new PageDescriptionContract {PageName = x.PageName, PageXmlId = x.PageXmlId})
                        .ToDictionary(x => x.Key, x => x.ToList());

                var booksTermResultsCount = m_bookVersionRepository.GetBooksTermResultsCount(guidList, termQueryCreator);
                bookTermResultsCount = booksTermResultsCount.ToDictionary(x => x.BookId, x => x.PagesCount);
            }

            var searchResultFullContext = new List<SearchResultContract>();


            foreach (var searchResult in searchResults.SearchResults)
            {
                var localResult = Mapper.Map<SearchResultContract>(resultDictionary[searchResult.BookXmlId]);
                localResult.TotalHitCount = searchResult.TotalHitCount;
                localResult.Results = searchResult.Results;
                localResult.PageCount = resultPageCountDictionary[localResult.BookId];

                if (bookTermResults != null)
                {
                    List<PageDescriptionContract> pageDescriptionContracts;
                    long termResultsCount;
                    localResult.TermsPageHits = bookTermResults.TryGetValue(localResult.BookId, out pageDescriptionContracts)
                        ? pageDescriptionContracts
                        : new List<PageDescriptionContract>();
                    localResult.TermsPageHitsCount = bookTermResultsCount.TryGetValue(localResult.BookId, out termResultsCount)
                        ? Convert.ToInt32(termResultsCount)
                        : 0;
                }

                searchResultFullContext.Add(localResult);
            }

            return searchResultFullContext;
        }

        public List<BookContract> GetBooksByBookType(BookTypeContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.FindBookVersionsByTypeWithAuthors(type);
            return Mapper.Map<List<BookContract>>(bookVersions);
        }

        public IList<BookContract> Search(BookTypeContract category, SearchDestinationContract searchBy, string query)
        {
            var type = Mapper.Map<BookTypeEnum>(category);
            IList<BookVersion> bookList;

            switch (searchBy)
            {
                case SearchDestinationContract.Author:
                    bookList = m_bookRepository.SearchByAuthorAndBookType(query, type);
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
            query = query.Replace("[", "[[]"); // escape character
            return string.Format("%{0}%", query);
        }

        public IList<string> GetTypeaheadAuthors(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastAuthors(PrefetchRecordCount);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadAuthors(query, PrefetchRecordCount);
        }

        public IList<UserContract> GetTypeaheadUsers(string query)
        {
            IList<User> users = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                users = m_userRepository.GetLastUsers(PrefetchRecordCount);
            }
            else
            {
                query = PrepareQuery(query);
                users = m_userRepository.GetTypeaheadUsers(query, PrefetchRecordCount);
            }

            var userContracts = Mapper.Map<IList<UserContract>>(users);
            return userContracts;
        }

        public IList<GroupContract> GetTypeaheadGroups(string query)
        {
            IList<Group> groups = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                groups = m_permissionRepository.GetLastGroups(PrefetchRecordCount);
            }
            else
            {
                query = PrepareQuery(query);
                groups = m_permissionRepository.GetTypeaheadGroups(query, PrefetchRecordCount);
            }

            var groupContracts = Mapper.Map<IList<GroupContract>>(groups);
            return groupContracts;
        }

        public IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookTypeContract)
        {
            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastAuthorsByBookType(PrefetchRecordCount, bookType);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadAuthorsByBookType(query, bookType, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadTitles(string query) //TODO HACK should be secure directly in queryover
        {
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTitles(PrefetchRecordCount);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadTitles(query, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookTypeContract, IList<int> selectedCategoryIds,
            IList<long> selectedBookIds)
        {
            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            var bookIdList = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, bookType);

            m_authorizationManager.FilterBookIdList(ref bookIdList);


            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTitlesByBookType(PrefetchRecordCount, bookType, bookIdList);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadTitlesByBookType(query, bookType, bookIdList, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadTermsByBookType(string query, BookTypeEnumContract bookTypeContract, IList<int> selectedCategoryIds,
            IList<long> selectedBookIds)
        {
            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            var bookIdList = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, bookType);

            m_authorizationManager.FilterBookIdList(ref bookIdList);

            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetTermsByBookType(PrefetchRecordCount, bookType, bookIdList);

            query = PrepareQuery(query);
            return m_bookRepository.GetTermsByBookType(query, bookType, bookIdList, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query,
            BookTypeEnumContract? bookType)
        {
            var currentBookType = Mapper.Map<BookTypeEnum>(bookType);
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, currentBookType);

            m_authorizationManager.FilterBookIdList(ref bookIds);

            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTypeaheadHeadwords(PrefetchRecordCount, bookIds);

            query = string.Format("{0}%", query);
            query = EscapeQuery(query);
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
                        BookTitle = headword.BookTitle,
                        BookXmlId = headword.BookGuid,
                        BookVersionXmlId = headword.BookVersionId
                    };
                    dictionaryList.Add(dictionaryContract.BookXmlId, dictionaryContract);
                }

                var bookInfoContract = new HeadwordBookInfoContract
                {
                    BookXmlId = headword.BookGuid,
                    EntryXmlId = headword.XmlEntryId,
                    Image = headword.Image
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

        private IList<long> GetCompleteBookIdList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, BookTypeEnum? currentBookType)
        {
            //Get all books for current module
            if (selectedBookIds == null && selectedCategoryIds == null)
            {
                if(currentBookType.HasValue)
                    return m_categoryRepository.GetAllBookIdsByBookType(currentBookType.Value);
                return m_categoryRepository.GetAllBookIds();
            }

            if (selectedCategoryIds != null && selectedCategoryIds.Count > 0)
            {
                var bookIdsFromCategory = m_categoryRepository.GetBookIdsFromCategory(selectedCategoryIds);
                return selectedBookIds == null ? bookIdsFromCategory : bookIdsFromCategory.Concat(selectedBookIds).ToList();
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


        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds, BookTypeEnumContract bookType)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, Mapper.Map<BookTypeEnum>(bookType));

            m_authorizationManager.FilterBookIdList(ref bookIds); //TODO HACK when all filtered, statement below returns all

            return bookIds == null
                ? m_bookVersionRepository.GetHeadwordCount()
                : m_bookVersionRepository.GetHeadwordCount(bookIds);
        }

        public HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int start, int count,
            BookTypeEnumContract bookType)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, Mapper.Map<BookTypeEnum>(bookType));

            m_authorizationManager.FilterBookIdList(ref bookIds); //TODO HACK when all filtered, statement below returns all

            var databaseResult = bookIds == null
                ? m_bookVersionRepository.GetHeadwordList(start, count)
                : m_bookVersionRepository.GetHeadwordList(start, count, bookIds);
            var result = ConvertHeadwordSearchToContract(databaseResult);

            return result;
        }

        public long GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, BookTypeEnumContract bookType)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, Mapper.Map<BookTypeEnum>(bookType));

            m_authorizationManager.FilterBookIdList(ref bookIds); //TODO HACK when all filtered, statement below returns all

            query = string.Format("{0}%", query);
            query = EscapeQuery(query);

            var bookHeadword = m_bookVersionRepository.FindFirstHeadword(bookIds, query);
            return m_bookVersionRepository.GetHeadwordRowNumberById(bookIds, bookHeadword.BookVersion.Book.Guid,
                bookHeadword.XmlEntryId);
        }

        public long GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId,
            BookTypeEnumContract bookType)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds, Mapper.Map<BookTypeEnum>(bookType));

            m_authorizationManager.FilterBookIdList(ref bookIds); //TODO HACK when all filtered, statement below returns all

            var resultRowNumber = m_bookVersionRepository.GetHeadwordRowNumberById(bookIds, headwordBookId, headwordEntryXmlId);
            if (resultRowNumber == 0)
                throw new ArgumentException("Headword not found.");

            return resultRowNumber;
        }

        public int SearchHeadwordByCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias,
            DictionarySearchTarget searchTarget)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
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
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
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

        public HeadwordListContract SearchHeadwordByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias, DictionarySearchTarget searchTarget)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
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
                var resultHeadwords = m_bookVersionRepository.GetHeadwordListBySearchCriteria(bookGuidList, headwordQueryCreator,
                    resultCriteria.Start.Value, resultCriteria.Count.Value);
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
            var bookInfoList = m_bookVersionRepository.GetBookVersionsByGuid(resultContract.BookList.Keys.ToList());
            var bookInfoContracts = Mapper.Map<IList<DictionaryContract>>(bookInfoList);
            var bookDictionary = bookInfoContracts.ToDictionary(x => x.BookXmlId, x => x);
            resultContract.BookList = bookDictionary;

            return resultContract;
        }

        public string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid, string xmlEntryId,
            OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            var bookVersion = m_bookRepository.GetLastVersionForBookWithType(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookType);
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;
            var dictionaryEntryText = m_searchServiceClient.GetDictionaryEntryFromSearch(criterias,
                bookGuid, bookVersion.VersionId, xmlEntryId, transformationName, resultFormat, transformationLevel);

            return dictionaryEntryText;
        }

        public PageListContract GetSearchEditionsPageList(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);
            return m_searchServiceClient.GetSearchEditionsPageList(criterias);
        }

        public CorpusSearchResultContractList GetCorpusSearchResults(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;

            if (nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().FirstOrDefault() == null)
            {
                var databaseSearchResult =
                    m_bookVersionRepository.SearchByCriteriaQuery(new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery,
                        filteredCriterias.MetadataParameters));

                if (databaseSearchResult.Count == 0)
                {
                    return new CorpusSearchResultContractList();
                }

                var resultContract = new ResultRestrictionCriteriaContract
                {
                    ResultBooks = databaseSearchResult
                };
                nonMetadataCriterias.Add(resultContract);
            }

            if (filteredCriterias.NonMetadataCriterias.All(x => x.Key == CriteriaKey.ResultRestriction || x.Key == CriteriaKey.Result))
            {
                // Search only in SQL
                var resultRestriction = nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().First();
                var guidListRestriction = resultRestriction.ResultBooks.Select(x => x.Guid).ToList();
                var resultBookVersions = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidListRestriction);
                var results = Mapper.Map<IList<CorpusSearchResultContract>>(resultBookVersions);
                var resultList = new CorpusSearchResultContractList
                {
                    SearchResults = results
                };

                return resultList;
            }

            // Fulltext search
            var searchResults = m_searchServiceClient.GetCorpusSearchResults(nonMetadataCriterias);

            var guidList = searchResults.SearchResults.Select(x => x.BookXmlId).ToList();
            var result = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidList);

            var resultDictionary = result.ToDictionary(x => x.Book.Guid);

            var searchResultFullContext = new List<CorpusSearchResultContract>();

            foreach (var searchResult in searchResults.SearchResults)
            {
                var localResult = Mapper.Map<CorpusSearchResultContract>(resultDictionary[searchResult.BookXmlId]);
                localResult.Notes = searchResult.Notes;
                localResult.PageResultContext = searchResult.PageResultContext;
                localResult.VerseResultContext = searchResult.VerseResultContext;
                searchResultFullContext.Add(localResult);
            }

            var searchList = new CorpusSearchResultContractList
            {
                SearchResults = searchResultFullContext
            };

            return searchList;
        }

        public int GetCorpusSearchResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
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

            return m_searchServiceClient.GetCorpusSearchResultsCount(nonMetadataCriterias);
        }

        public string GetEditionPageFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookXmlId, string pageXmlId,
            OutputFormatEnumContract resultFormat)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var bookVersion = m_bookRepository.GetLastVersionForBookWithType(bookXmlId);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat,
                bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;
            var pageText = m_searchServiceClient.GetEditionPageFromSearch(criterias,
                bookXmlId, bookVersion.VersionId, pageXmlId, transformationName, resultFormat, transformationLevel);

            return pageText;
        }

        public AudioBookSearchResultContractList GetAudioBookSearchResults(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;

            if (nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().FirstOrDefault() == null)
            {
                var databaseSearchResult =
                    m_bookVersionRepository.SearchByCriteriaQuery(new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery,
                        filteredCriterias.MetadataParameters));

                if (databaseSearchResult.Count == 0)
                {
                    return new AudioBookSearchResultContractList();
                }

                var resultContract = new ResultRestrictionCriteriaContract
                {
                    ResultBooks = databaseSearchResult
                };
                nonMetadataCriterias.Add(resultContract);
            }

            // Search only in SQL
            var resultRestriction = nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().First();
            var guidListRestriction = resultRestriction.ResultBooks.Select(x => x.Guid).ToList();
            var resultBookVersions = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidListRestriction);

            var audibookList = new List<AudioBookSearchResultContract>();

            foreach (var bookVersion in resultBookVersions)
            {
                var tracks = m_bookVersionRepository.GetTracksForBookVersion(bookVersion.Id);

                var audioBook = Mapper.Map<AudioBookSearchResultContract>(bookVersion);
                audioBook.Tracks = Mapper.Map<IList<TrackContract>>(tracks);

                audibookList.Add(audioBook);
            }

            var resultList = new AudioBookSearchResultContractList
            {
                Results = audibookList
            };

            return resultList;
        }

        public int GetAudioBooksSearchResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var criterias = searchCriterias.ToList();
            m_authorizationManager.AuthorizeCriteria(criterias);

            var filteredCriterias = FilterSearchCriterias(criterias);
            var creator = new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery, filteredCriterias.MetadataParameters);
            var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(creator);
            return databaseSearchResult.Count;
        }

        private class FilteredCriterias
        {
            public List<SearchCriteriaQuery> ConjunctionQuery { get; set; }
            public List<SearchCriteriaContract> NonMetadataCriterias { get; set; }
            public List<SearchCriteriaContract> MetadataCriterias { get; set; }
            public Dictionary<string, object> MetadataParameters { get; set; }
            public ResultCriteriaContract ResultCriteria { get; set; }
        }
    }
}