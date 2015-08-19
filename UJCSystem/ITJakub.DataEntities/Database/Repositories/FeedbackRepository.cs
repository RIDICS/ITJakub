using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.Shared.Contracts.Searching.Criteria;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class FeedbackRepository : NHibernateTransactionalDao<Feedback>
    {
        public FeedbackRepository(ISessionManager sessManager) : base(sessManager)
        {
        }


        [Transaction(TransactionMode.Requires)]
        public virtual IList<Feedback> GetFeedbacks(List<FeedbackCategoryEnum> categories, int? start, int? count)
        {
            using (var session = GetSession())
            {
                var query = session.QueryOver<Feedback>();
                if (categories != null)
                {
                    query.Where(x => x.Category.IsIn(categories));
                }

                if (start.HasValue)
                {
                    query.Skip(start.Value);
                }

                if (count.HasValue)
                {
                    query.Take(count.Value);
                }

                return query.Fetch(x => x.User).Eager.List<Feedback>();
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual int GetFeedbacksCount(List<FeedbackCategoryEnum> categories)
        {
            using (var session = GetSession())
            {
                var query =  session.QueryOver<Feedback>();

                if (categories != null)
                {
                    query.Where(x => x.Category.IsIn(categories));
                }

                return query.RowCount();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFeedback(long feedbackId)
        {
            using (var session = GetSession())
            {
                var feedback = session.QueryOver<Feedback>().Where(fb => fb.Id == feedbackId).SingleOrDefault<Feedback>();
                session.Delete(feedback);
            }
        }
    }
}