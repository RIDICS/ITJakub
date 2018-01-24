using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class CreateTokenWork : UnitOfWorkBase<long>
    {
        private readonly LemmatizationRepository m_repository;
        private readonly string m_token;
        private readonly string m_description;

        public CreateTokenWork(LemmatizationRepository lemmatizationRepository, string token, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_token = token;
            m_description = description;
        }

        protected override long ExecuteWorkImplementation()
        {
            var newToken = new Token
            {
                Text = m_token,
                Description = m_description
            };

            var id = m_repository.Create(newToken);
            return (long)id;
        }
    }
}