using System.Collections.Generic;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core
{
    public class SearchManager
    {
        private readonly BookRepository m_bookRepository;
        private readonly CategoryRepository m_categoryRepository;

        public SearchManager(BookRepository bookRepository, CategoryRepository categoryRepository)
        {
            m_bookRepository = bookRepository;
            m_categoryRepository = categoryRepository;
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

        public List<SearchResultContract> GetBooksByBookType(BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.FindBooksLastVersionsByBookType(type);
            return Mapper.Map<List<SearchResultContract>>(bookVersions);
        }

        public List<SearchResultContract> SearchBooksWithBookType(string term, BookTypeEnumContract bookType)
        {
            var type = Mapper.Map<BookTypeEnum>(bookType);
            var bookVersions = m_bookRepository.SearchByTitleAndBookType(term, type);
            return Mapper.Map<List<SearchResultContract>>(bookVersions);
        }
    }
}