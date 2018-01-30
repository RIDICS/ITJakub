using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class RemoveHyperCanonicalFormWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_canonicalFormId;

        public RemoveHyperCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long canonicalFormId) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_canonicalFormId = canonicalFormId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var canonicalForm = m_repository.FindById<CanonicalForm>(m_canonicalFormId);
            canonicalForm.HyperCanonicalForm = null;
            m_repository.Update(canonicalForm);
        }
    }
}