using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class GroupRepository : NHibernateTransactionalDao<Group>
    {
        public GroupRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group LoadGroupWithDetails(long id)
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

        [Transaction(TransactionMode.Requires)]
        public virtual Group FindByEnterCode(string enterCode)
        {
            using (var session = GetSession())
            {
                return
                    session.CreateCriteria<Group>()
                        .Add(Restrictions.Eq("EnterCode", enterCode))
                        .SetFetchMode("Members", FetchMode.Join)
                        .SetFetchMode("Author", FetchMode.Join)
                        .UniqueResult<Group>();
            }
        }


        [Transaction(TransactionMode.Requires)]
        public override object Create(Group group)
        {
            try
            {
                return base.Create(group);
            }
            catch (DataException)
            {
                throw new CreateEntityFailedException();
            }
        }
    }
}