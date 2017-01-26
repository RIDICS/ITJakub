using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Person
{
    public class GetAuthorAutocompleteWork : UnitOfWorkBase<IList<OriginalAuthor>>
    {
        private readonly PersonRepository m_personRepository;
        private readonly string m_query;
        private readonly int m_count;

        public GetAuthorAutocompleteWork(PersonRepository personRepository, string query, int count) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_query = query;
            m_count = count;
        }

        protected override IList<OriginalAuthor> ExecuteWorkImplementation()
        {
            return m_personRepository.GetAuthorAutocomplete(m_query, m_count);
        }
    }
}