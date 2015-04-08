using System.Collections.Generic;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.ITJakubService.Core
{
    public class MobileManager
    {
        private readonly BookRepository m_bookRepository;

        public MobileManager(BookRepository bookRepository)
        {
            m_bookRepository = bookRepository;
        }

        public IList<BookContract> GetBooksByCategory(CategoryContract category)
        {
            var bookType = Mapper.Map<BookTypeEnum>(category);
            var bookList = m_bookRepository.FindBooksByBookType(bookType);

            return Mapper.Map<IList<BookContract>>(bookList);
        }
    }
}
