using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class HeadwordSearchManager
    {
        private readonly MetadataRepository m_metadataRepository;

        public HeadwordSearchManager(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public List<HeadwordContract> GetHeadwordSearchResultByStandardIds(List<HeadwordDictionaryEntryData> list)
        {
            throw new NotImplementedException();
        }

        public List<HeadwordContract> GetHeadwordSearchResultByExternalIds(List<HeadwordDictionaryEntryData> list)
        {
            var orderedResultList = new List<HeadwordContract>();
            foreach (var headwordDictionaryEntryData in list)
            {
                var headwordInfo = m_metadataRepository.InvokeUnitOfWork(x => x.GetHeadwordWithFetchByExternalId(headwordDictionaryEntryData.ProjectExternalId, headwordDictionaryEntryData.HeadwordExternalId));
                var headwordContract = Mapper.Map<HeadwordContract>(headwordInfo);
                orderedResultList.Add(headwordContract);
            }

            return orderedResultList;
        }
    }
}
