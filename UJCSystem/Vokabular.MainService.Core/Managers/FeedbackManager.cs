using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Portal;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class FeedbackManager
    {
        private readonly PortalRepository m_portalRepository;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;
        private readonly IMapper m_mapper;

        public FeedbackManager(PortalRepository portalRepository, AuthenticationManager authenticationManager, UserDetailManager userDetailManager, IMapper mapper)
        {
            m_portalRepository = portalRepository;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
            m_mapper = mapper;
        }

        public long CreateFeedback(CreateFeedbackContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var resultId = new CreateFeedbackWork(m_mapper, m_portalRepository, data, FeedbackType.Generic, userId).Execute();
            return resultId;
        }

        public long CreateAnonymousFeedback(CreateAnonymousFeedbackContract data)
        {
            var resultId = new CreateFeedbackWork(m_mapper, m_portalRepository, data, FeedbackType.Generic).Execute();
            return resultId;
        }

        public long CreateHeadwordFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var resultId = new CreateFeedbackWork(m_mapper, m_portalRepository, data, FeedbackType.Headword, userId, resourceVersionId).Execute();
            return resultId;
        }

        public long CreateAnonymousHeadwordFeedback(long resourceVersionId, CreateAnonymousFeedbackContract data)
        {
            var resultId = new CreateFeedbackWork(m_mapper, m_portalRepository, data, FeedbackType.Headword, null, resourceVersionId).Execute();
            return resultId;
        }

        public PagedResultList<FeedbackContract> GetFeedbackList(int? start, int? count, FeedbackSortEnumContract sort, SortDirectionEnumContract sortDirection, IList<FeedbackCategoryEnumContract> filterCategories)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var sortValue = m_mapper.Map<FeedbackSortEnum>(sort);
            var filterCategoryValues = m_mapper.Map<List<FeedbackCategoryEnum>>(filterCategories);

            var result = m_portalRepository.InvokeUnitOfWork(repository =>
            {
                var dbFeedbacks = repository.GetFeedbackList(startValue, countValue, sortValue, sortDirection, filterCategoryValues);

                var headwordFeedbackIds = dbFeedbacks.List.Where(x => x.FeedbackType == FeedbackType.Headword)
                    .Select(x => x.Id);
                repository.FetchHeadwordFeedbacks(headwordFeedbackIds);

                return dbFeedbacks;
            });

            return new PagedResultList<FeedbackContract>
            {
                List = m_userDetailManager.AddUserDetails(m_mapper.Map<List<FeedbackContract>>(result.List)),
                TotalCount = result.Count,
            };
        }

        public void DeleteFeedback(long feedbackId)
        {
            new DeleteFeedbackWork(m_portalRepository, feedbackId).Execute();
        }
    }
}
