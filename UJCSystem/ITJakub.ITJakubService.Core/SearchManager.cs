using System.Collections.Generic;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.Core.Search;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using NHibernate;
using NHibernate.Criterion;
using MobileContracts = ITJakub.MobileApps.MobileContracts;

namespace ITJakub.ITJakubService.Core
{
    public class SearchManager
    {
        private readonly BookRepository m_bookRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly SearchCriteriaDirector m_searchCriteriaDirector;

        public SearchManager(BookRepository bookRepository, CategoryRepository categoryRepository, SearchCriteriaDirector searchCriteriaDirector)
        {
            m_bookRepository = bookRepository;
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

        public void SearchByCriteria(List<SearchCriteriaContract> searchCriterias)
        {
            var databaseCriteria = DetachedCriteria.For<BookVersion>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                m_searchCriteriaDirector.ProcessCriteria(searchCriteriaContract, databaseCriteria);
            }
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
    }
}