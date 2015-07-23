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
using ITJakub.Shared.Contracts.Searching;
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
            var nonMetadataCriterias = new List<SearchCriteriaContract>();
            var conjunction = new List<SearchCriteriaQuery>();
            var metadataParameters = new Dictionary<string, object>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    var criteriaQuery = m_searchCriteriaDirector.ProcessCriteria(searchCriteriaContract,
                        metadataParameters);
                    conjunction.Add(criteriaQuery);
                }
                else
                {
                    nonMetadataCriterias.Add(searchCriteriaContract);
                }
            }

            return new FilteredCriterias
            {
                MetadataParameters = metadataParameters,
                NonMetadataCriterias = nonMetadataCriterias,
                ConjunctionQuery = conjunction
            };
        }

        public IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var filteredCriterias = FilterSearchCriterias(searchCriterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;

            var databaseSearchResult =
                m_bookVersionRepository.SearchByCriteriaQuery(
                    new SearchCriteriaQueryCreator(filteredCriterias.ConjunctionQuery,
                        filteredCriterias.MetadataParameters));
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

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookTypeContract)
        {
            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            if (string.IsNullOrWhiteSpace(query))
                return m_bookRepository.GetLastTitlesByBookType(PrefetchRecordCount, bookType);

            query = PrepareQuery(query);
            return m_bookRepository.GetTypeaheadTitlesByBookType(query, bookType, PrefetchRecordCount);
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            string query)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);
            if (bookIds.Count == 0)
                bookIds = null;

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

        public HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            int start, int end)
        {
            var bookIds = GetCompleteBookIdList(selectedCategoryIds, selectedBookIds);

            var databaseResult = bookIds.Count == 0
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

        public int SearchHeadwordByCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias,
            DictionarySearchTarget searchTarget)
        {
            var filteredCriterias = FilterSearchCriterias(searchCriterias);
            var nonMetadataCriterias = filteredCriterias.NonMetadataCriterias;

            if (searchTarget == DictionarySearchTarget.Fulltext)
            {
                // TODO search in SearchService, from SQL get bookVersionPair
            }

            //TODO
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
            // TODO search in SQL and get bookVersionPair
            var databaseSearchResult =
                m_bookVersionRepository.GetBookVersionsByGuid(new List<string>
                {
                    "{08BE3E56-77D0-46C1-80BB-C1346B757BE5}",
                    "{1ADA5193-4375-4269-8222-D8BE81D597DB}"
                }).Select(x => new BookVersionPairContract
                {
                    Guid = x.Book.Guid,
                    VersionId = x.VersionId
                }).ToList();

            var resultRestrictionContract = new ResultRestrictionCriteriaContract
            {
                ResultBooks = databaseSearchResult
            };

            var fileteredCriterias = FilterSearchCriterias(searchCriterias);
            fileteredCriterias.NonMetadataCriterias.Add(resultRestrictionContract);

            var serializedResult =
                m_searchServiceClient.ListSearchDictionariesResults(fileteredCriterias.NonMetadataCriterias);
            var contract = new HeadwordListContract
            {
                HeadwordList = new List<HeadwordContract> //TODO
                {
                    new HeadwordContract
                    {
                        Headword = "Abc",
                        Dictionaries = new List<HeadwordBookInfoContract>
                        {
                            new HeadwordBookInfoContract {BookXmlId = "XmlIdKnihy1", EntryXmlId = "EntryId1"},
                            new HeadwordBookInfoContract {BookXmlId = "XmlIdKnihy2", EntryXmlId = "EntryId1"}
                        }
                    },
                    new HeadwordContract
                    {
                        Headword = "Defg",
                        Dictionaries = new List<HeadwordBookInfoContract>
                        {
                            new HeadwordBookInfoContract {BookXmlId = "XmlIdKnihy1", EntryXmlId = "EntryId1"},
                            new HeadwordBookInfoContract {BookXmlId = "XmlIdKnihy2", EntryXmlId = "EntryId1"},
                            new HeadwordBookInfoContract {BookXmlId = "XmlIdKnihy3", EntryXmlId = "EntryId3"}
                        }
                    }
                },
                BookList = new Dictionary<string, DictionaryContract>()
            };

            contract.BookList.Add("XmlIdKnihy1",
                new DictionaryContract
                {
                    BookXmlId = "XmlIdKnihy1",
                    BookVersionXmlId = "XmlIdVerze",
                    BookAcronym = "ES",
                    BookTitle = "Elektronický slovník"
                });
            contract.BookList.Add("XmlIdKnihy2",
                new DictionaryContract
                {
                    BookXmlId = "XmlIdKnihy2",
                    BookVersionXmlId = "XmlIdVerze",
                    BookAcronym = "StCS",
                    BookTitle = "Staročeský slovník"
                });
            contract.BookList.Add("XmlIdKnihy3",
                new DictionaryContract
                {
                    BookXmlId = "XmlIdKnihy3",
                    BookVersionXmlId = "XmlIdVerze",
                    BookAcronym = "ESSC",
                    BookTitle = "Slovník"
                });
            contract.BookList.Add("XmlIdKnihy4", null);

            return contract;
        }

        public string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid,
            string xmlEntryId, OutputFormatEnumContract resultFormat)
        {
            return "<div class='entryFree'><span class='bo'>Heslo</span></div>";
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
            public Dictionary<string, object> MetadataParameters { get; set; }
        }
    }
}