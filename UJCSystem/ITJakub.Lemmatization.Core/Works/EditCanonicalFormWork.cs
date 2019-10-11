using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class EditCanonicalFormWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_canonicalFormId;
        private readonly string m_text;
        private readonly CanonicalFormType m_type;
        private readonly string m_description;

        public EditCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long canonicalFormId, string text, CanonicalFormType type, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_canonicalFormId = canonicalFormId;
            m_text = text;
            m_type = type;
            m_description = description;
        }

        protected override void ExecuteWorkImplementation()
        {
            var canonicalForm = m_repository.FindById<CanonicalForm>(m_canonicalFormId);
            canonicalForm.Text = m_text;
            canonicalForm.Type = m_type;
            canonicalForm.Description = m_description;

            m_repository.Update(canonicalForm);
        }
    }
}