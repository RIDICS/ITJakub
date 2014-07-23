using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class GroupRepository: NHibernateTransactionalDao<Group>
    {
        public GroupRepository(ISessionManager sessManager): base(sessManager)
        {
        }

        public Group LoadGroupWithDetails(long id)
        {
            using (var session = GetSession())
            {
                return
                    session.CreateCriteria<Group>()
                        .Add(Restrictions.Eq(Projections.Id(), id))
                        .SetFetchMode("Members", FetchMode.Join)
                        .SetFetchMode("Author", FetchMode.Join)
                        .UniqueResult<Group>();
            }
        }
    }
}