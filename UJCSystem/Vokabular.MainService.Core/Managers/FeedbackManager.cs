using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.Portal;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class FeedbackManager
    {
        private readonly PortalRepository m_portalRepository;
        private readonly UserManager m_userManager;

        public FeedbackManager(PortalRepository portalRepository, UserManager userManager)
        {
            m_portalRepository = portalRepository;
            m_userManager = userManager;
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

        public long CreateResourceFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            var userId = m_userManager.GetCurrentUserId();
            var resultId = new CreateFeedbackWork(m_portalRepository, data, FeedbackType.Headword, userId, resourceVersionId).Execute();
            return resultId;
        }

        public long CreateAnonymousResourceFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            var resultId = new CreateFeedbackWork(m_portalRepository, data, FeedbackType.Headword, null, resourceVersionId).Execute();
            return resultId;
        }

        public PagedResultList<FeedbackContract> GetFeedbackList(int? start, int? count, FeedbackSortEnumContract sort, SortDirectionEnumContract sortDirection, IList<FeedbackCategoryEnumContract> filterCategoryIds)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteFeedback(long feedbackId)
        {
            throw new System.NotImplementedException();
        }
    }
}
