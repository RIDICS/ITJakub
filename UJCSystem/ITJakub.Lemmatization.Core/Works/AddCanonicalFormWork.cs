using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class AddCanonicalFormWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_tokenCharacteristicId;
        private readonly long m_canonicalFormId;

        public AddCanonicalFormWork(LemmatizationRepository lemmatizationRepository, long tokenCharacteristicId, long canonicalFormId) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_tokenCharacteristicId = tokenCharacteristicId;
            m_canonicalFormId = canonicalFormId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var tokenCharacteristic = m_repository.GetTokenCharacteristicWithCanonicalForms(m_tokenCharacteristicId);
            var cannonicalForm = m_repository.Load<CanonicalForm>(m_canonicalFormId);

            tokenCharacteristic.CanonicalForms.Add(cannonicalForm);
            m_repository.Update(tokenCharacteristic);
        }
    }
}