using NHibernate.Exceptions;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Person
{
    public class DeleteResponsiblePersonWork : UnitOfWorkBase
    {
        private readonly PersonRepository m_personRepository;
        private readonly int m_personId;

        public DeleteResponsiblePersonWork(PersonRepository personRepository, int personId) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_personId = personId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var item = m_personRepository.FindById<ResponsiblePerson>(m_personId);
            if (item == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            try
            {
                m_personRepository.Delete(item);
                m_personRepository.UnitOfWork.CurrentSession.Flush();
            }
            catch (GenericADOException)
            {
                throw new MainServiceException(MainServiceErrorCode.DeleteResourceProjectRelation, "Could not delete resource. Existing relation to Project?");
            }
        }
    }
}