using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.Person
{
    public class UpdateOriginalAuthorWork : UnitOfWorkBase
    {
        private readonly PersonRepository m_personRepository;
        private readonly int m_authorId;
        private readonly OriginalAuthorContract m_data;

        public UpdateOriginalAuthorWork(PersonRepository personRepository, int authorId, OriginalAuthorContract data) : base(personRepository)
        {
            m_personRepository = personRepository;
            m_authorId = authorId;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbAuthor = m_personRepository.FindById<OriginalAuthor>(m_authorId);
            if (dbAuthor == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            dbAuthor.FirstName = m_data.FirstName;
            dbAuthor.LastName = m_data.LastName;
            
            m_personRepository.Update(dbAuthor);
        }
    }
}