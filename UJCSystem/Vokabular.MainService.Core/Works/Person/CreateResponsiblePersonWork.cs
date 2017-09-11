using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Person
{
    public class CreateResponsiblePersonWork : UnitOfWorkBase<int>
    {
        private readonly PersonRepository m_personRepository;
        private readonly ResponsiblePersonContract m_data;

        public CreateResponsiblePersonWork(PersonRepository personRepository, ResponsiblePersonContract data) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var responsiblePerson = new ResponsiblePerson
            {
                FirstName = m_data.FirstName,
                LastName = m_data.LastName,
            };
            return (int) m_personRepository.Create(responsiblePerson);
        }
    }
}