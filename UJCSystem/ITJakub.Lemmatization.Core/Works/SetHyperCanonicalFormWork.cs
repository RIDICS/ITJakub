using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class SetHyperCanonicalFormWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_canonicalFormId;
        private readonly long m_hyperCanonicalFormId;

        public SetHyperCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long canonicalFormId, long hyperCanonicalFormId) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_canonicalFormId = canonicalFormId;
            m_hyperCanonicalFormId = hyperCanonicalFormId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var canonicalForm = m_repository.FindById<CanonicalForm>(m_canonicalFormId);
            var hyperCanonicalForm = m_repository.Load<HyperCanonicalForm>(m_hyperCanonicalFormId);

            canonicalForm.HyperCanonicalForm = hyperCanonicalForm;
            m_repository.Update(canonicalForm);
        }
    }
}