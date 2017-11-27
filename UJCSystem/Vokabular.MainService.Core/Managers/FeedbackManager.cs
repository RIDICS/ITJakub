using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class FeedbackManager
    {
        private readonly PortalRepository m_portalRepository;

        public FeedbackManager(PortalRepository portalRepository)
        {
            m_portalRepository = portalRepository;
        }

        public long CreateFeedback(CreateFeedbackContract data)
        {
            throw new System.NotImplementedException();
        }

        public long CreateAnonymousFeedback(CreateAnonymousFeedbackContract data)
        {
            throw new System.NotImplementedException();
        }

        public long CreateResourceFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            throw new System.NotImplementedException();
        }

        public long CreateAnonymousResourceFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            throw new System.NotImplementedException();
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
