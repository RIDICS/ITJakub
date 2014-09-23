using ITJakub.DataEntities.Database.Entities;
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
    }
}
