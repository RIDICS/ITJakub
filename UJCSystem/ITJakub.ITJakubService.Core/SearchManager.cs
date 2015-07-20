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
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;
using MobileContracts = ITJakub.MobileApps.MobileContracts;

namespace ITJakub.ITJakubService.Core
{
    public class SearchManager
    {
        private readonly BookRepository m_bookRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly MetadataSearchCriteriaDirector m_searchCriteriaDirector;
        private readonly SearchServiceClient m_searchServiceClient;
        private const int PrefetchRecordCount = 5;

        public SearchManager(BookRepository bookRepository, BookVersionRepository bookVersionRepository, CategoryRepository categoryRepository, MetadataSearchCriteriaDirector searchCriteriaDirector, SearchServiceClient searchServiceClient)
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
                Books = Mapper.Map<IList<BookContract>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        public IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var nonMetadataCriterias = new List<SearchCriteriaContract>();
            var conjunction = new List<SearchCriteriaQuery>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    var criteriaQuery = m_searchCriteriaDirector.ProcessCriteria(searchCriteriaContract);
                    conjunction.Add(criteriaQuery);
                }
                else
                {
                    nonMetadataCriterias.Add(searchCriteriaContract);
                }
            }
            
            IList<BookVersionPairContract> databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(conjunction);
            if (databaseSearchResult.Count == 0)
                return new List<SearchResultContract>();

            var resultContract = new ResultRestrictionCriteriaContract
            {
                ResultBooks = databaseSearchResult
            };
            nonMetadataCriterias.Add(resultContract);

            m_searchServiceClient.ListSearchEditionsResults(nonMetadataCriterias);

            //TODO return correct results

            var guidList = databaseSearchResult.Select(x => x.Guid);
            var result = m_bookVersionRepository.GetBookVersionsByGuid(guidList);

            return Mapper.Map<List<SearchResultContract>>(result);
        }

        public List<SearchResultContract> GetBooksByBookType(BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.FindBooksLastVersionsByBookType(type);
            return Mapper.Map<List<SearchResultContract>>(bookVersions);
        }

        public List<MobileContracts.BookContract> GetBooksByBookType(MobileContracts.BookTypeContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.FindBooksLastVersionsByBookType(type);
            return Mapper.Map<List<MobileContracts.BookContract>>(bookVersions);
        }

        public List<SearchResultContract> SearchBooksWithBookType(string term, BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.SearchByTitleAndBookType(term, type);
            return Mapper.Map<List<SearchResultContract>>(bookVersions);
        }

        public IList<MobileContracts.BookContract> Search(MobileContracts.BookTypeContract category, MobileContracts.SearchDestinationContract searchBy, string query)
        {
            var type = Mapper.Map<BookTypeEnum>(category);
            IList<BookVersion> bookList = null;

            switch (searchBy)
            {
                case MobileContracts.SearchDestinationContract.Author:
                    //TODO search by author
                    break;
                default:
                    bookList = m_bookRepository.SearchByTitleAndBookType(query, type);
                    break;
            }
            return Mapper.Map<IList<MobileContracts.BookContract>>(bookList);
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

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookTypeContract)
        {
            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTitlesByBookType(PrefetchRecordCount, bookType);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadTitlesByBookType(query, bookType, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);
            if (bookIds.Count == 0)
                bookIds = null;

            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTypeaheadHeadwords(PrefetchRecordCount, bookIds);

            query = string.Format("{0}%", query);
            return m_bookRepository.GetTypeaheadHeadwords(query, PrefetchRecordCount, bookIds);
        }

        private IList<HeadwordContract> ConvertHeadwordSearchToContract(IList<HeadwordSearchResult> databaseResult)
        {
            var resultList = new List<HeadwordContract>();
            var headwordContract = new HeadwordContract();
            foreach (var headword in databaseResult)
            {
                var bookInfoContract = new HeadwordBookInfoContract
                {
                    BookAcronym = headword.BookAcronym,
                    BookTitle = headword.BookTitle,
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
                        Dictionaries = new List<HeadwordBookInfoContract> { bookInfoContract },
                        Headword = headword.Headword
                    };
                    resultList.Add(headwordContract);
                }
            }
            return resultList;
        }

        private IList<long> GetCompleteBookIdList(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var bookIdsFromCategory = m_categoryRepository.GetBookIdsFromCategory(selectedCategoryIds);
            return selectedBookIds != null
                ? bookIdsFromCategory.Concat(selectedBookIds).ToList()
                : bookIdsFromCategory;
        }

        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            return bookIds.Count == 0
                ? m_bookVersionRepository.GetHeadwordCount()
                : m_bookVersionRepository.GetHeadwordCount(bookIds);
        }

        public IList<HeadwordContract> GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int start, int end)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            var databaseResult = bookIds.Count == 0
                ? m_bookVersionRepository.GetHeadwordList(start, end)
                : m_bookVersionRepository.GetHeadwordList(start, end, bookIds);
            var result = ConvertHeadwordSearchToContract(databaseResult);

            return result;
        }
        
        public int GetHeadwordPageNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            return m_bookVersionRepository.GetPageNumberForHeadword(bookIds, query);
        }

        public HeadwordSearchResultContract GetHeadwordSearchResultCount(string query)
        {
            query = string.Format("%{0}%", query);
            var databaseResult = m_bookVersionRepository.GetCountOfSearchHeadword(query, new [] {"{08BE3E56-77D0-46C1-80BB-C1346B757BE5}"});

            return null; //TODO
        }

        public IList<HeadwordContract> SearchHeadword(string query, IList<string> dictionaryGuidList, int page, int pageSize)
        {
            query = string.Format("%{0}%", query);
            var databaseResult = m_bookVersionRepository.SearchHeadword(query, dictionaryGuidList, page, pageSize);
            var resultList = ConvertHeadwordSearchToContract(databaseResult);

            return resultList;
        }
    }
}