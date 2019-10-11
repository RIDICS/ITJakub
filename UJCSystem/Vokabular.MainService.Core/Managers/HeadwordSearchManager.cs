using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class HeadwordSearchManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly IMapper m_mapper;

        public HeadwordSearchManager(ResourceRepository resourceRepository, IMapper mapper)
        {
            m_resourceRepository = resourceRepository;
            m_mapper = mapper;
        }

        public List<HeadwordContract> GetHeadwordSearchResultByStandardIds(List<HeadwordDictionaryEntryData> list)
        {
            throw new NotImplementedException();
        }

        public List<HeadwordContract> GetHeadwordSearchResultByExternalIds(List<HeadwordDictionaryEntryData> list, ProjectTypeEnum projectType)
        {
            var orderedResultList = new List<HeadwordContract>();
            foreach (var headwordDictionaryEntryData in list)
            {
                var headwordInfo = m_resourceRepository.InvokeUnitOfWork(x => x.GetHeadwordWithFetchByExternalId(headwordDictionaryEntryData.ProjectExternalId, headwordDictionaryEntryData.HeadwordExternalId, projectType));
                var headwordContract = m_mapper.Map<HeadwordContract>(headwordInfo);
                orderedResultList.Add(headwordContract);
            }

            return orderedResultList;
        }
    }
}
