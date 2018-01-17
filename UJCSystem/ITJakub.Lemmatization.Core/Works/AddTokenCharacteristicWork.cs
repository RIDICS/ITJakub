using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class AddTokenCharacteristicWork : UnitOfWorkBase<long>
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_tokenId;
        private readonly string m_morphologicalCharacteristic;
        private readonly string m_description;

        public AddTokenCharacteristicWork(LemmatizationRepository lemmatizationRepository, long tokenId, string morphologicalCharacteristic, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_tokenId = tokenId;
            m_morphologicalCharacteristic = morphologicalCharacteristic;
            m_description = description;
        }

        protected override long ExecuteWorkImplementation()
        {
            var tokenEntity = m_repository.Load<Token>(m_tokenId);
            var newTokenCharacteristic = new TokenCharacteristic
            {
                MorphologicalCharakteristic = m_morphologicalCharacteristic,
                Description = m_description,
                Token = tokenEntity
            };

            var id = m_repository.Create(newTokenCharacteristic);
            return (long)id;
        }
    }
}
