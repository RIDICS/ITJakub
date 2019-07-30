using System.Collections.Generic;
using AutoMapper;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class CreateCanonicalFormWork : UnitOfWorkBase<long>
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_tokenCharacteristicId;
        private readonly CanonicalFormTypeContract m_type;
        private readonly string m_text;
        private readonly string m_description;

        public CreateCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_tokenCharacteristicId = tokenCharacteristicId;
            m_type = type;
            m_text = text;
            m_description = description;
        }

        protected override long ExecuteWorkImplementation()
        {
            var tokenCharacteristic = m_repository.Load<TokenCharacteristic>(m_tokenCharacteristicId);
            var canonicalFormType = Mapper.Map<CanonicalFormType>(m_type);
            var newCanonicalForm = new CanonicalForm
            {
                Type = canonicalFormType,
                Text = m_text,
                Description = m_description,
                CanonicalFormFor = new List<TokenCharacteristic> { tokenCharacteristic }
            };

            var id = m_repository.Create(newCanonicalForm);
            return (long)id;
        }
    }
}