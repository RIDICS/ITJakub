using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.Core.Search;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using MobileContracts = ITJakub.MobileApps.MobileContracts;

namespace ITJakub.ITJakubService.Core
{
    public class SearchManager
    {
        private readonly BookRepository m_bookRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly SearchCriteriaDirector m_searchCriteriaDirector;
        private const int PrefetchRecordCount = 5;

        public SearchManager(BookRepository bookRepository, BookVersionRepository bookVersionRepository, CategoryRepository categoryRepository, SearchCriteriaDirector searchCriteriaDirector)
        {
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
            m_categoryRepository = categoryRepository;
            m_searchCriteriaDirector = searchCriteriaDirector;
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
            var categories = m_categoryRepository.FindCategoriesByBookType(type);

            return new BookTypeSearchResultContract
            {
                BookType = bookType,
                Books = Mapper.Map<IList<BookContract>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        public IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            var conjunction = new List<SearchCriteriaQuery>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                var criteriaQuery = m_searchCriteriaDirector.ProcessCriteria(searchCriteriaContract);
                conjunction.Add(criteriaQuery);
            }
            
            var databaseSearchResult = m_bookVersionRepository.SearchByCriteriaQuery(conjunction);
            
            // TODO search in eXist

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
        
        public IList<string> GetTypeaheadDictionaryEntries(string query)
        {
            return new List<string> {"TODO"}; // TODO search dictionary entries
        }
    }
}