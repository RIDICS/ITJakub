using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class FeedbackRepository : NHibernateTransactionalDao<Feedback>
    {
        public FeedbackRepository(ISessionManager sessManager) : base(sessManager)
        {
        }


        [Transaction(TransactionMode.Requires)]
        public virtual IList<Feedback> GetFeedbacks()
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Feedback>().List<Feedback>();
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual int GetFeedbacksCount()
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Feedback>().RowCount();
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