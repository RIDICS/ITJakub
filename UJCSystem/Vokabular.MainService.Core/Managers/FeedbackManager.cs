using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.Portal;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class FeedbackManager
    {
        private const int DefaultStart = 0;
        private const int DefaultCount = 20;
        private const int MaxCount = 200;
        private readonly PortalRepository m_portalRepository;
        private readonly UserManager m_userManager;

        public FeedbackManager(PortalRepository portalRepository, UserManager userManager)
        {
            m_portalRepository = portalRepository;
            m_userManager = userManager;
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

        public long CreateFeedback(CreateFeedbackContract data)
        {
            var userId = m_userManager.GetCurrentUserId();
            var resultId = new CreateFeedbackWork(m_portalRepository, data, FeedbackType.Generic, userId).Execute();
            return resultId;
        }

        public long CreateAnonymousFeedback(CreateAnonymousFeedbackContract data)
        {
            var resultId = new CreateFeedbackWork(m_portalRepository, data, FeedbackType.Generic).Execute();
            return resultId;
        }

        public long CreateHeadwordFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            var userId = m_userManager.GetCurrentUserId();
            var resultId = new CreateFeedbackWork(m_portalRepository, data, FeedbackType.Headword, userId, resourceVersionId).Execute();
            return resultId;
        }

        public long CreateAnonymousHeadwordFeedback(long resourceVersionId, CreateAnonymousFeedbackContract data)
        {
            var resultId = new CreateFeedbackWork(m_portalRepository, data, FeedbackType.Headword, null, resourceVersionId).Execute();
            return resultId;
        }

        public PagedResultList<FeedbackContract> GetFeedbackList(int? start, int? count, FeedbackSortEnumContract sort, SortDirectionEnumContract sortDirection, IList<FeedbackCategoryEnumContract> filterCategories)
        {
            var startValue = GetStart(start);
            var countValue = GetCount(count);
            var sortValue = Mapper.Map<FeedbackSortEnum>(sort);
            var filterCategoryValues = Mapper.Map<List<FeedbackCategoryEnum>>(filterCategories);
            var result = m_portalRepository.InvokeUnitOfWork(x => x.GetFeedbackList(startValue, countValue, sortValue, sortDirection, filterCategoryValues));

            return new PagedResultList<FeedbackContract>
            {
                List = Mapper.Map<List<FeedbackContract>>(result.List),
                TotalCount = result.Count,
            };
        }

        public void DeleteFeedback(long feedbackId)
        {
            new DeleteFeedbackWork(m_portalRepository, feedbackId).Execute();
        }
    }
}
