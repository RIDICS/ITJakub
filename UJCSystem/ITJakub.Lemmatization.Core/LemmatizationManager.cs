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

            var id = m_repository.Create(newToken);
            return (long) id;
        }

        public IList<TokenCharacteristicDetailContract> GetTokenCharacteristic(long tokenId)
        {
            var result = m_repository.GetTokenCharacteristicDetail(tokenId);
            return Mapper.Map<IList<TokenCharacteristicDetailContract>>(result);
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

            var id = m_repository.Create(newTokenCharacteristic);
            return (long) id;
        }

        public IList<CanonicalFormTypeaheadContract> GetTypeaheadCannonicalForm(CanonicalFormTypeContract type, string query)
        {
            query = EscapeQuery(query);
            var canonicalFormType = Mapper.Map<CanonicalFormType>(type);
            var result = m_repository.GetTypeaheadCannonicalForm(canonicalFormType, query, PrefetchRecordCount);
            return Mapper.Map<IList<CanonicalFormTypeaheadContract>>(result);
        }

        public IList<HyperCanonicalFormContract> GetTypeaheadHyperCannonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            query = EscapeQuery(query);
            var formType = Mapper.Map<HyperCanonicalFormType>(type);
            var result = m_repository.GetTypeaheadHyperCannonicalForm(formType, query, PrefetchRecordCount);
            return Mapper.Map<IList<HyperCanonicalFormContract>>(result);
        }

        public long CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            var tokenCharacteristic = m_repository.Load<TokenCharacteristic>(tokenCharacteristicId);
            var canonicalFormType = Mapper.Map<CanonicalFormType>(type);
            var newCanonicalForm = new CanonicalForm
            {
                Type = canonicalFormType,
                Text = text,
                Description = description,
                CanonicalFormFor = new List<TokenCharacteristic> {tokenCharacteristic}
            };

            var id = m_repository.Create(newCanonicalForm);
            return (long) id;
        }

        public void AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            var tokenCharacteristic = m_repository.GetTokenCharacteristicWithCanonicalForms(tokenCharacteristicId);
            var cannonicalForm = m_repository.Load<CanonicalForm>(canonicalFormId);

            tokenCharacteristic.CanonicalForms.Add(cannonicalForm);
            m_repository.Update(tokenCharacteristic);
        }

        public void SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId)
        {
            var canonicalForm = m_repository.FindById<CanonicalForm>(canonicalFormId);
            var hyperCanonicalForm = m_repository.Load<HyperCanonicalForm>(hyperCanonicalFormId);

            canonicalForm.HyperCanonicalForm = hyperCanonicalForm;
            m_repository.Update(canonicalForm);
        }

        public long CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description)
        {
            var canonicalForm = m_repository.Load<CanonicalForm>(canonicalFormId);
            var hyperCanonicalFormType = Mapper.Map<HyperCanonicalFormType>(type);
            var hyperCanonicalForm = new HyperCanonicalForm
            {
                Type = hyperCanonicalFormType,
                Text = text,
                Description = description,
                CanonicalForms = new List<CanonicalForm> { canonicalForm }
            };

            var id = m_repository.Create(hyperCanonicalForm);
            return (long) id;
        }

        public void EditToken(long tokenId, string description)
        {
            var token = m_repository.FindById<Token>(tokenId);
            token.Description = description;

            m_repository.Update(token);
        }

        public void EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description)
        {
            var tokenCharacteristic = m_repository.FindById<TokenCharacteristic>(tokenCharacteristicId);
            tokenCharacteristic.MorphologicalCharakteristic = morphologicalCharacteristic;
            tokenCharacteristic.Description = description;

            m_repository.Update(tokenCharacteristic);
        }

        public void EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description)
        {
            var canonicalFormType = Mapper.Map<CanonicalFormType>(type);
            var canonicalForm = m_repository.FindById<CanonicalForm>(canonicalFormId);
            canonicalForm.Text = text;
            canonicalForm.Type = canonicalFormType;
            canonicalForm.Description = description;

            m_repository.Update(canonicalForm);
        }

        public void EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description)
        {
            var hyperCanonicalFormType = Mapper.Map<HyperCanonicalFormType>(type);
            var hyperCanonicalForm = m_repository.FindById<HyperCanonicalForm>(hyperCanonicalFormId);
            hyperCanonicalForm.Text = text;
            hyperCanonicalForm.Type = hyperCanonicalFormType;
            hyperCanonicalForm.Description = description;

            m_repository.Update(hyperCanonicalForm);
        }

        public int GetTokenCount()
        {
            return m_repository.GetTokenCount();
        }

        public IList<TokenContract> GetTokenList(int start, int count)
        {
            var result = m_repository.GetTokenList(start, count);
            var resultContract = Mapper.Map<IList<TokenContract>>(result);
            return resultContract;
        }

        public IList<long> GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            var result = m_repository.GetCanonicalFormIdList(hyperCanonicalFormId);
            return result;
        }

        public InverseCanonicalFormContract GetCanonicalFormDetail(long canonicalFormId)
        {
            var result = m_repository.GetCanonicalFormDetail(canonicalFormId);
            var contract = Mapper.Map<InverseCanonicalFormContract>(result);
            return contract;
        }

        public TokenContract GetToken(long tokenId)
        {
            var result = m_repository.FindById<Token>(tokenId);
            var contract = Mapper.Map<TokenContract>(result);
            return contract;
        }
    }
}
