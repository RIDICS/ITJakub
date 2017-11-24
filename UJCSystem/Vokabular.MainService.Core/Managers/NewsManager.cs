using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;

namespace Vokabular.MainService.Core.Managers
{
    public class NewsManager
    {
        private const int DefaultStart = 0;
        private const int DefaultCount = 20;
        private const int MaxCount = 200;
        private readonly PortalRepository m_portalRepository;

        public NewsManager(PortalRepository portalRepository)
        {
            m_portalRepository = portalRepository;
        }

        private int GetStart(int? start)
        {
            return start ?? DefaultStart;
        }

        private int GetCount(int? count)
        {
            return count != null
                ? Math.Min(count.Value, MaxCount)
                : DefaultCount;
        }

        public PagedResultList<NewsSyndicationItemContract> GetNewsSyndicationItems(int? start, int? count, NewsTypeEnumContract? itemType)
        {
            var startValue = GetStart(start);
            var countValue = GetCount(count);
            var syndicationItemType = Mapper.Map<SyndicationItemType?>(itemType);

            var result = m_portalRepository.InvokeUnitOfWork(x =>
                x.GetNewsSyndicationItems(startValue, countValue, syndicationItemType));

            return new PagedResultList<NewsSyndicationItemContract>
            {
                List = Mapper.Map<List<NewsSyndicationItemContract>>(result.List),
                TotalCount = result.Count
            };
        }

        public long CreateNewsSyndicationItem(CreateNewsSyndicationItemContract data)
        {
            throw new System.NotImplementedException();
        }
    }
}
