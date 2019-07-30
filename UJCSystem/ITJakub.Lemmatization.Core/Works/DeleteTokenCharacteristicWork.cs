using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class DeleteTokenCharacteristicWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_tokenCharacteristicId;

        public DeleteTokenCharacteristicWork(LemmatizationRepository lemmatizationRepository, long tokenCharacteristicId) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_tokenCharacteristicId = tokenCharacteristicId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var tokenCharacteristic = m_repository.Load<TokenCharacteristic>(m_tokenCharacteristicId);
            m_repository.Delete(tokenCharacteristic);
        }
    }
}