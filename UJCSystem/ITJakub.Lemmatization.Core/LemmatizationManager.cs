using System.Collections.Generic;
using AutoMapper;
using ITJakub.Lemmatization.Core.Works;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.Core
{
    public class LemmatizationManager
    {
        private readonly LemmatizationRepository m_repository;
        private readonly IMapper m_mapper;
        private const int PrefetchRecordCount = 10;

        public LemmatizationManager(LemmatizationRepository repository, IMapper mapper)
        {
            m_repository = repository;
            m_mapper = mapper;
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
            return m_mapper.Map<IList<TokenContract>>(result);
        }

        public long CreateToken(string token, string description)
        {
            return new CreateTokenWork(m_repository, token, description).Execute();
        }

        public IList<TokenCharacteristicDetailContract> GetTokenCharacteristic(long tokenId)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.GetTokenCharacteristicDetail(tokenId));
            return m_mapper.Map<IList<TokenCharacteristicDetailContract>>(result);
        }

        public long AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            return new AddTokenCharacteristicWork(m_repository, tokenId, morphologicalCharacteristic, description).Execute();
        }

        public IList<CanonicalFormTypeaheadContract> GetTypeaheadCannonicalForm(CanonicalFormTypeContract type, string query)
        {
            query = EscapeQuery(query);
            var canonicalFormType = m_mapper.Map<CanonicalFormType>(type);
            var result = m_repository.InvokeUnitOfWork(x => x.GetTypeaheadCannonicalForm(canonicalFormType, query, PrefetchRecordCount));
            return m_mapper.Map<IList<CanonicalFormTypeaheadContract>>(result);
        }

        public IList<HyperCanonicalFormContract> GetTypeaheadHyperCannonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            query = EscapeQuery(query);
            var formType = m_mapper.Map<HyperCanonicalFormType>(type);
            var result = m_repository.InvokeUnitOfWork(x => x.GetTypeaheadHyperCannonicalForm(formType, query, PrefetchRecordCount));
            return m_mapper.Map<IList<HyperCanonicalFormContract>>(result);
        }

        public long CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            var canonicalFormType = m_mapper.Map<CanonicalFormType>(type);
            return new CreateCanonicalFormWork(m_repository, tokenCharacteristicId, canonicalFormType, text, description).Execute();
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
            var hyperCanonicalFormType = m_mapper.Map<HyperCanonicalFormType>(type);
            return new CreateHyperCanonicalFormWork(m_repository, canonicalFormId, hyperCanonicalFormType, text, description).Execute();
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
            var canonicalFormType = m_mapper.Map<CanonicalFormType>(type);
            new EditCanonicalFormWork(m_repository, canonicalFormId, text, canonicalFormType, description).Execute();
        }

        public void EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description)
        {
            var hyperCanonicalFormType = m_mapper.Map<HyperCanonicalFormType>(type);
            new EditHyperCanonicalFormWork(m_repository, hyperCanonicalFormId, text, hyperCanonicalFormType, description).Execute();
        }

        public int GetTokenCount()
        {
            return m_repository.InvokeUnitOfWork(x => x.GetTokenCount());
        }

        public IList<TokenContract> GetTokenList(int start, int count)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.GetTokenList(start, count));
            var resultContract = m_mapper.Map<IList<TokenContract>>(result);
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
            var contract = m_mapper.Map<InverseCanonicalFormContract>(result);
            return contract;
        }

        public TokenContract GetToken(long tokenId)
        {
            var result = m_repository.InvokeUnitOfWork(x => x.FindById<Token>(tokenId));
            var contract = m_mapper.Map<TokenContract>(result);
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
