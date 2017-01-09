using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Person
{
    public class GetResponsibleTypeListWork : UnitOfWorkBase<IList<ResponsibleType>>
    {
        private readonly PersonRepository m_personRepository;

        public GetResponsibleTypeListWork(PersonRepository personRepository) : base(personRepository.UnitOfWork)
        {
            m_personRepository = personRepository;
        }

        protected override IList<ResponsibleType> ExecuteWorkImplementation()
        {
            return m_personRepository.GetResponsibleTypeList();
        }
    }
}