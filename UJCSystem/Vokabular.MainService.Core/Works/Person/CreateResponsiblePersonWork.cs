using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Person
{
    public class CreateResponsiblePersonWork : UnitOfWorkBase<int>
    {
        private readonly PersonRepository m_personRepository;
        private readonly NewResponsiblePersonContract m_data;

        public CreateResponsiblePersonWork(PersonRepository personRepository, NewResponsiblePersonContract data) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            throw new System.InvalidOperationException("Database model was changed. UI and logic update is required");
            var responsibleTypes = new List<ResponsibleType>();
            foreach (var responsibleTypeId in m_data.ResponsibleTypeIdList)
            {
                var responsibleType = m_personRepository.Load<ResponsibleType>(responsibleTypeId);
                responsibleTypes.Add(responsibleType);
            }

            var responsiblePerson = new ResponsiblePerson
            {
                FirstName = m_data.FirstName,
                LastName = m_data.LastName,
                //ResponsibleTypes = responsibleTypes
            };
            return (int) m_personRepository.Create(responsiblePerson);
        }
    }
}