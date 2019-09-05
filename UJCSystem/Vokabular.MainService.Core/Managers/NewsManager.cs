using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Portal;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class NewsManager
    {
        private readonly PortalRepository m_portalRepository;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;
        private readonly IMapper m_mapper;

        public NewsManager(PortalRepository portalRepository, AuthenticationManager authenticationManager, UserDetailManager userDetailManager, IMapper mapper)
        {
            m_portalRepository = portalRepository;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
            m_mapper = mapper;
        }

        public PagedResultList<NewsSyndicationItemContract> GetNewsSyndicationItems(int? start, int? count, NewsTypeEnumContract? itemType)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var syndicationItemType = m_mapper.Map<SyndicationItemType?>(itemType);

            var result = m_portalRepository.InvokeUnitOfWork(x =>
                x.GetNewsSyndicationItems(startValue, countValue, syndicationItemType));

            return new PagedResultList<NewsSyndicationItemContract>
            {
                List = m_userDetailManager.AddUserDetails(m_mapper.Map<List<NewsSyndicationItemContract>>(result.List)),
                TotalCount = result.Count
            };
        }

        public long CreateNewsSyndicationItem(CreateNewsSyndicationItemContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateNewsWork(m_portalRepository, data, userId, m_mapper);
            var resultId = work.Execute();
            return resultId;
        }
    }
}
