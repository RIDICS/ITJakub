using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Person
{
    public class CreateOrUpdateResponsiblePersonWork : UnitOfWorkBase<int>
    {
        private readonly PersonRepository m_personRepository;
        private readonly int? m_responsiblePersonId;
        private readonly ResponsiblePersonContract m_data;

        public CreateOrUpdateResponsiblePersonWork(PersonRepository personRepository, int? responsiblePersonId, ResponsiblePersonContract data) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_responsiblePersonId = responsiblePersonId;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var responsiblePerson = m_responsiblePersonId != null
                ? m_personRepository.FindById<ResponsiblePerson>(m_responsiblePersonId.Value)
                : new ResponsiblePerson();

            if (responsiblePerson == null)
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");

            responsiblePerson.FirstName = m_data.FirstName;
            responsiblePerson.LastName = m_data.LastName;
            
            m_personRepository.Save(responsiblePerson);

            return responsiblePerson.Id;
        }
    }
}