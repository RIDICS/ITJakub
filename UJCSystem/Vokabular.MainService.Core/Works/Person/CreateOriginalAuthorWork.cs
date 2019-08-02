using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Person
{
    public class CreateOriginalAuthorWork : UnitOfWorkBase<int>
    {
        private readonly PersonRepository m_personRepository;
        private readonly OriginalAuthorContract m_data;

        public CreateOriginalAuthorWork(PersonRepository personRepository, OriginalAuthorContract data) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var author = new OriginalAuthor
            {
                FirstName = m_data.FirstName,
                LastName = m_data.LastName
            };

            return (int) m_personRepository.Create(author);
        }
    }
}