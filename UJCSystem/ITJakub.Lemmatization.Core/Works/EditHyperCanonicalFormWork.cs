using AutoMapper;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class EditHyperCanonicalFormWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_hyperCanonicalFormId;
        private readonly string m_text;
        private readonly HyperCanonicalFormTypeContract m_type;
        private readonly string m_description;

        public EditHyperCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_hyperCanonicalFormId = hyperCanonicalFormId;
            m_text = text;
            m_type = type;
            m_description = description;
        }

        protected override void ExecuteWorkImplementation()
        {
            var hyperCanonicalFormType = Mapper.Map<HyperCanonicalFormType>(m_type);
            var hyperCanonicalForm = m_repository.FindById<HyperCanonicalForm>(m_hyperCanonicalFormId);
            hyperCanonicalForm.Text = m_text;
            hyperCanonicalForm.Type = hyperCanonicalFormType;
            hyperCanonicalForm.Description = m_description;

            m_repository.Update(hyperCanonicalForm);
        }
    }
}