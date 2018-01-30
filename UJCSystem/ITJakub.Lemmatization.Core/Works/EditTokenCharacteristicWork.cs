using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.Core.Works
{
    public class EditTokenCharacteristicWork : UnitOfWorkBase
    {
        private readonly LemmatizationRepository m_repository;
        private readonly long m_tokenCharacteristicId;
        private readonly string m_morphologicalCharacteristic;
        private readonly string m_description;

        public EditTokenCharacteristicWork(LemmatizationRepository lemmatizationRepository, long tokenCharacteristicId, string morphologicalCharacteristic, string description) : base(lemmatizationRepository)
        {
            m_repository = lemmatizationRepository;
            m_tokenCharacteristicId = tokenCharacteristicId;
            m_morphologicalCharacteristic = morphologicalCharacteristic;
            m_description = description;
        }

        protected override void ExecuteWorkImplementation()
        {
            var tokenCharacteristic = m_repository.FindById<TokenCharacteristic>(m_tokenCharacteristicId);
            tokenCharacteristic.MorphologicalCharakteristic = m_morphologicalCharacteristic;
            tokenCharacteristic.Description = m_description;

            m_repository.Update(tokenCharacteristic);
        }
    }
}