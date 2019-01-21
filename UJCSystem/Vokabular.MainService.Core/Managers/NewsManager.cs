using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Portal;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;

namespace Vokabular.MainService.Core.Managers
{
    public class NewsManager
    {
        private readonly PortalRepository m_portalRepository;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly UserDetailManager m_userDetailManager;

        public NewsManager(PortalRepository portalRepository, AuthorizationManager authorizationManager, UserDetailManager userDetailManager)
        {
            m_portalRepository = portalRepository;
            m_authorizationManager = authorizationManager;
            m_userDetailManager = userDetailManager;
        }

        public PagedResultList<NewsSyndicationItemContract> GetNewsSyndicationItems(int? start, int? count, NewsTypeEnumContract? itemType)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var syndicationItemType = Mapper.Map<SyndicationItemType?>(itemType);

            var result = m_portalRepository.InvokeUnitOfWork(x =>
                x.GetNewsSyndicationItems(startValue, countValue, syndicationItemType));

            return new PagedResultList<NewsSyndicationItemContract>
            {
                List = m_userDetailManager.GetUserDetailContracts(Mapper.Map<List<NewsSyndicationItemContract>>(result.List)),
                TotalCount = result.Count
            };
        }

        public long CreateNewsSyndicationItem(CreateNewsSyndicationItemContract data)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            var work = new CreateNewsWork(m_portalRepository, data, userId);
            var resultId = work.Execute();
            return resultId;
        }
    }
}
