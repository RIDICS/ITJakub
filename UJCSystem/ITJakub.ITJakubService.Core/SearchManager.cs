using System.Collections.Generic;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;
using BookContract = ITJakub.Shared.Contracts.BookContract;
using MobileBookContract = ITJakub.MobileApps.MobileContracts.BookContract;
using MobileBookTypeContract = ITJakub.MobileApps.MobileContracts.BookTypeContract;
using MobileSearchDestinationContract = ITJakub.MobileApps.MobileContracts.SearchDestinationContract;

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
            var books = m_bookRepository.FindBooksByBookType(type);
            var categories = m_categoryRepository.FindCategoriesByBookType(type);

            return new BookTypeSearchResultContract
            {
                BookType = bookType,
                Books = Mapper.Map<IList<BookContract>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        public IList<MobileBookContract> GetBooksByBookType(MobileBookTypeContract category)
        {
            var bookType = Mapper.Map<BookTypeEnum>(category);
            var bookList = m_bookRepository.FindBooksByBookType(bookType);

            return Mapper.Map<IList<MobileBookContract>>(bookList);
        }

        public IList<MobileBookContract> Search(MobileBookTypeContract category, MobileSearchDestinationContract searchBy, string query)
        {
            IList<BookVersion> bookList = null;
            switch (searchBy)
            {
                case MobileSearchDestinationContract.Author:
                    //TODO
                    break;
                default:
                    //TODO
                    bookList = m_bookRepository.SearchByTitle(query);
                    break;
            }
            return Mapper.Map<IList<MobileBookContract>>(bookList);
        }
    }
}