using System.Collections.Generic;
using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core
{
    public class BookManager
    {
        private readonly BookRepository m_bookRepository;

        public BookManager(BookRepository bookRepository)
        {
            m_bookRepository = bookRepository;
        }

        public void CreateBook(string bookGuid, string name, string author)
        {
            m_bookRepository.CreateBook(bookGuid, name, author);
        }

        public void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds)
        {
            m_bookRepository.AssignAuthorsToBook(bookGuid, bookVersionGuid, authorIds);
        }
    }
}