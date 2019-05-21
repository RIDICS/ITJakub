using System.Collections.Generic;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class FeedbackRepository : NHibernateTransactionalDao<Feedback>
    {
        public FeedbackRepository(ISessionManager sessManager) : base(sessManager)
        {
        }


        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<Feedback> GetFeedbacks(List<FeedbackCategoryEnum> categories, FeedbackSortEnum sortCriteria, bool sortAsc, int? start, int? count)
        {
            using (var session = GetSession())
            {
                var query = session.QueryOver<Feedback>();

                var queryOrderBuilder = query.OrderBy(Projections.Property(sortCriteria.ToString()));

                query = sortAsc ? queryOrderBuilder.Asc : queryOrderBuilder.Desc;

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

                return query.Fetch(SelectMode.Fetch, x => x.User).List<Feedback>();
            }
        }


        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual void DeleteFeedback(long feedbackId)
        {
            using (var session = GetSession())
            {
                var feedback = session.QueryOver<Feedback>().Where(fb => fb.Id == feedbackId).SingleOrDefault<Feedback>();
                session.Delete(feedback);
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<HeadwordFeedback> GetHeadwordFeedbacksById(IEnumerable<long> feedbackIds)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<HeadwordFeedback>()
                    .WhereRestrictionOn(x => x.Id).IsInG(feedbackIds)
                    .Fetch(SelectMode.Fetch, x => x.BookHeadword)
                    .Fetch(SelectMode.Fetch, x => x.BookHeadword.BookVersion)
                    .List();
            }
        }
    }
}