using System.Collections.Generic;
using AutoMapper;
using ITJakub.Lemmatization.DataEntities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Lemmatization.Core
{
    public class LemmatizationManager
    {
        private readonly LemmatizationRepository m_repository;
        private const int PrefetchRecordCount = 10;

        public LemmatizationManager(LemmatizationRepository repository)
        {
            m_repository = repository;
        }

        private string EscapeQuery(string query)
        {
            return query.Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
        }

        public IList<TokenContract> GetTypeaheadToken(string query)
        {
            query = EscapeQuery(query);
            var result = m_repository.GetTypeaheadToken(query, PrefetchRecordCount);
            return Mapper.Map<IList<TokenContract>>(result);
        }

        public long CreateToken(string token, string description)
        {
            var newToken = new Token
            {
                Text = token,
                Description = description
            };

            newToken = m_repository.Create(newToken);
            return newToken.Id;
        }

        public IList<TokenCharacteristicContract> GetTokenCharacteristic(long tokenId)
        {
            var result = m_repository.GetTokenCharacteristicDetail(tokenId);
            return Mapper.Map<IList<TokenCharacteristicContract>>(result);
        }

        public long AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            var tokenEntity = m_repository.Load<Token>(tokenId);
            var newTokenCharacteristic = new TokenCharacteristic
            {
                MorphologicalCharakteristic = morphologicalCharacteristic,
                Description = description,
                Token = tokenEntity
            };

            newTokenCharacteristic = m_repository.Create(newTokenCharacteristic);
            return newTokenCharacteristic.Id;
        }

        public IList<CanonicalFormContract> GetTypeaheadCannonicalForm(string query)
        {
            query = EscapeQuery(query);
            var result = m_repository.GetTypeaheadCannonicalForm(query, PrefetchRecordCount);
            return Mapper.Map<IList<CanonicalFormContract>>(result);
        }

        public long CreateCanonicalForm(CanonicalFormTypeContract type, string text, string description)
        {
            var formType = Mapper.Map<CanonicalFormType>(type);
            var newCanonicalForm = new CanonicalForm
            {
                Type = formType,
                Text = text,
                Description = description
            };

            newCanonicalForm = m_repository.Create(newCanonicalForm);
            return newCanonicalForm.Id;
        }

        public void AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            var tokenCharacteristic = m_repository.FindById<TokenCharacteristic>(tokenCharacteristicId);
            var cannonicalForm = m_repository.Load<CanonicalForm>(canonicalFormId);

            tokenCharacteristic.CanonicalForms.Add(cannonicalForm);
            m_repository.Update(tokenCharacteristic);
        }
    }
}
