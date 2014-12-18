using System.Collections.Generic;
using System.Linq;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core
{
    public class SearchManager
    {
        private readonly BookRepository m_bookRepository;

        public SearchManager(BookRepository bookRepository)
        {
            m_bookRepository = bookRepository;
        }

        public List<SearchResultContract> Search(string term)
        {
            var bookVersionResults = m_bookRepository.SearchByTitle(term);
            return bookVersionResults.Select(bookVersion => new SearchResultContract
            {
                BookId = bookVersion.Book.Guid,
                BookType = bookVersion.Book.BookType.ToString(),
                Name = bookVersion.Title
            }).ToList();
        }
    }
}