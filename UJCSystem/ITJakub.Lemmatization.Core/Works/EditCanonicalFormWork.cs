using AutoMapper;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class EditCanonicalFormWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_canonicalFormId;
        private readonly string m_text;
        private readonly CanonicalFormTypeContract m_type;
        private readonly string m_description;

        public EditCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long canonicalFormId, string text, CanonicalFormTypeContract type, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_canonicalFormId = canonicalFormId;
            m_text = text;
            m_type = type;
            m_description = description;
        }

        protected override void ExecuteWorkImplementation()
        {
            var canonicalFormType = Mapper.Map<CanonicalFormType>(m_type);
            var canonicalForm = m_repository.FindById<CanonicalForm>(m_canonicalFormId);
            canonicalForm.Text = m_text;
            canonicalForm.Type = canonicalFormType;
            canonicalForm.Description = m_description;

            m_repository.Update(canonicalForm);
        }
    }
}