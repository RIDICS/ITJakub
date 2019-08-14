using System.Net;
using NHibernate.Exceptions;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Person
{
    public class DeleteOriginalAuthorWork : UnitOfWorkBase
    {
        private readonly PersonRepository m_personRepository;
        private readonly int m_authorId;

        public DeleteOriginalAuthorWork(PersonRepository personRepository, int authorId) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_authorId = authorId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbAuthor = m_personRepository.FindById<OriginalAuthor>(m_authorId);
            if (dbAuthor == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            try
            {
                m_personRepository.Delete(dbAuthor);
                m_personRepository.UnitOfWork.CurrentSession.Flush();
            }
            catch (GenericADOException exception)
            {
                throw new HttpErrorCodeException("Could not delete resource. Existing relation to Project?", exception, HttpStatusCode.BadRequest);
            }
        }
    }
}