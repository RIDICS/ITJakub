using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class TaskRepository : NHibernateTransactionalDao<Task>
    {
        public TaskRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Task> LoadTasksWithDetailsByApplication(Application application)
        {
            using (var session = GetSession())
            {
                return
                    session.CreateCriteria<Task>()
                        .Add(Restrictions.Eq("Application", application))
                        .SetFetchMode("Groups", FetchMode.Join)
                        .SetFetchMode("Author", FetchMode.Join)
                        .List<Task>();
            }
        }
    }
}