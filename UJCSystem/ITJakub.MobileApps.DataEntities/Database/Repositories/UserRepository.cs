using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class UserRepository : NHibernateTransactionalDao<User>
    {
        public UserRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User LoadUserWithDetails(long id)
        {
            using (var session = GetSession())
            {
                return
                    session.CreateCriteria<User>()
                        .Add(Restrictions.Eq(Projections.Id(), id))
                        .SetFetchMode("MemberOfGroups", FetchMode.Join)
                        .SetFetchMode("CreatedGroups", FetchMode.Join)
                        .SetFetchMode("CreatedTasks", FetchMode.Join)
                        .UniqueResult<User>();
            }
        }
    }
}