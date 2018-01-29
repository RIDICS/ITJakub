using System.Collections.Generic;
using AutoMapper;
using ITJakub.Lemmatization.Core.Works;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.DataEntities.Database.UnitOfWork;

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
            var result = m_repository.InvokeUnitOfWork(x => x.GetTypeaheadToken(query, PrefetchRecordCount));
            return Mapper.Map<IList<TokenContract>>(result);
        }

        public long CreateToken(string token, string description)
        {
            return new CreateTokenWork(m_repository, token, description).Execute();
        }

        public IList<TokenCharacteristicDetailContract> GetTokenCharacteristic(long tokenId)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.GetTokenCharacteristicDetail(tokenId));
            return Mapper.Map<IList<TokenCharacteristicDetailContract>>(result);
        }

        public long AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            return new AddTokenCharacteristicWork(m_repository, tokenId, morphologicalCharacteristic, description).Execute();
        }

        public IList<CanonicalFormTypeaheadContract> GetTypeaheadCannonicalForm(CanonicalFormTypeContract type, string query)
        {
            query = EscapeQuery(query);
            var canonicalFormType = Mapper.Map<CanonicalFormType>(type);
            var result = m_repository.InvokeUnitOfWork(x => x.GetTypeaheadCannonicalForm(canonicalFormType, query, PrefetchRecordCount));
            return Mapper.Map<IList<CanonicalFormTypeaheadContract>>(result);
        }

        public IList<HyperCanonicalFormContract> GetTypeaheadHyperCannonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            query = EscapeQuery(query);
            var formType = Mapper.Map<HyperCanonicalFormType>(type);
            var result = m_repository.InvokeUnitOfWork(x => x.GetTypeaheadHyperCannonicalForm(formType, query, PrefetchRecordCount));
            return Mapper.Map<IList<HyperCanonicalFormContract>>(result);
        }

        public long CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            return new CreateCanonicalFormWork(m_repository, tokenCharacteristicId, type, text, description).Execute();
        }

        public void AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            new AddCanonicalFormWork(m_repository, tokenCharacteristicId, canonicalFormId).Execute();
        }

        public void SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId)
        {
            new SetHyperCanonicalFormWork(m_repository, canonicalFormId, hyperCanonicalFormId).Execute();
        }

        public long CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description)
        {
            return new CreateHyperCanonicalFormWork(m_repository, canonicalFormId, type, text, description).Execute();
        }

        public void EditToken(long tokenId, string description)
        {
            new EditTokenWork(m_repository, tokenId, description).Execute();
        }

        public void EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description)
        {
            new EditTokenCharacteristicWork(m_repository, tokenCharacteristicId, morphologicalCharacteristic, description).Execute();
        }

        public void EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description)
        {
            new EditCanonicalFormWork(m_repository, canonicalFormId, text, type, description).Execute();
        }

        public void EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description)
        {
            new EditHyperCanonicalFormWork(m_repository, hyperCanonicalFormId, text, type, description).Execute();
        }

        public int GetTokenCount()
        {
            return m_repository.InvokeUnitOfWork(x => x.GetTokenCount());
        }

        public IList<TokenContract> GetTokenList(int start, int count)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.GetTokenList(start, count));
            var resultContract = Mapper.Map<IList<TokenContract>>(result);
            return resultContract;
        }

        public IList<long> GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.GetCanonicalFormIdList(hyperCanonicalFormId));
            return result;
        }

        public InverseCanonicalFormContract GetCanonicalFormDetail(long canonicalFormId)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.GetCanonicalFormDetail(canonicalFormId));
            var contract = Mapper.Map<InverseCanonicalFormContract>(result);
            return contract;
        }

        public TokenContract GetToken(long tokenId)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.FindById<Token>(tokenId));
            var contract = Mapper.Map<TokenContract>(result);
            return contract;
        }

        public void DeleteTokenCharacteristic(long tokenCharacteristicId)
        {
            new DeleteTokenCharacteristicWork(m_repository, tokenCharacteristicId).Execute();
        }

        public void RemoveCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            new RemoveCanonicalFormWork(m_repository, tokenCharacteristicId, canonicalFormId).Execute();
        }

        public void RemoveHyperCanonicalForm(long canonicalFormId)
        {
            new RemoveHyperCanonicalFormWork(m_repository, canonicalFormId).Execute();
        }
    }
}
