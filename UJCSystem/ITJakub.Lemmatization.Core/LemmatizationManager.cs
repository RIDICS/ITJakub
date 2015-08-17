using System.Collections.Generic;
using AutoMapper;
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

        public IList<LemmatizationTypeaheadContract> GetTypeaheadToken(string query)
        {
            var result = m_repository.GetTypeaheadToken(query, PrefetchRecordCount);
            return Mapper.Map<IList<LemmatizationTypeaheadContract>>(result);
        }
    }
}
