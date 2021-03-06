﻿using System.Collections.Generic;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class CreateHyperCanonicalFormWork : UnitOfWorkBase<long>
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_canonicalFormId;
        private readonly HyperCanonicalFormType m_type;
        private readonly string m_text;
        private readonly string m_description;

        public CreateHyperCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long canonicalFormId, HyperCanonicalFormType type, string text, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_canonicalFormId = canonicalFormId;
            m_type = type;
            m_text = text;
            m_description = description;
        }

        protected override long ExecuteWorkImplementation()
        {
            var canonicalForm = m_repository.Load<CanonicalForm>(m_canonicalFormId);
            var hyperCanonicalForm = new HyperCanonicalForm
            {
                Type = m_type,
                Text = m_text,
                Description = m_description,
                CanonicalForms = new List<CanonicalForm> { canonicalForm }
            };

            var id = m_repository.Create(hyperCanonicalForm);
            return (long)id;
        }
    }
}