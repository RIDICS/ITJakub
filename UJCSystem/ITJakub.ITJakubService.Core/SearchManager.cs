using System.Collections.Generic;
using AutoMapper;
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
            return Mapper.Map<List<SearchResultContract>>(bookVersionResults);
        }
    }
}