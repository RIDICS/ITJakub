using System.Collections.Generic;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class AuthorManager
    {
        private readonly AuthorRepository m_authorRepository;

        public AuthorManager(AuthorRepository authorRepository)
        {
            m_authorRepository = authorRepository;
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            var authors = m_authorRepository.GetAllAuthors();
            return Mapper.Map<IList<Author>,IList<AuthorDetailContract>>(authors);
        }
    }
}