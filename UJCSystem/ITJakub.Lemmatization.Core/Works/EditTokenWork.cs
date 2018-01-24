using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class EditTokenWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_tokenId;
        private readonly string m_description;

        public EditTokenWork(LemmatizationRepository lemmatizationRepository, long tokenId, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_tokenId = tokenId;
            m_description = description;
        }

        protected override void ExecuteWorkImplementation()
        {
            var token = m_repository.FindById<Token>(m_tokenId);
            token.Description = m_description;

            m_repository.Update(token);
        }
    }
}