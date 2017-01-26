using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Person
{
    public class CreateResponsibleTypeWork : UnitOfWorkBase<int>
    {
        private readonly PersonRepository m_personRepository;
        private readonly ResponsibleTypeContract m_data;

        public CreateResponsibleTypeWork(PersonRepository personRepository, ResponsibleTypeContract data) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var typeEnum = Mapper.Map<ResponsibleTypeEnum>(m_data.Type);
            var responsibleType = new ResponsibleType
            {
                Text = m_data.Text,
                Type = typeEnum
            };
            return (int) m_personRepository.Create(responsibleType);
        }
    }
}