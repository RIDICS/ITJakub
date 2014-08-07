using System.Collections.Generic;
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
    public class GroupRepository : NHibernateTransactionalDao
    {
        public GroupRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group LoadGroupWithDetails(long id)
        {
            using (ISession session = GetSession())
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
            using (ISession session = GetSession())
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
        public virtual  object Create(Group group)
        {
            try
            {
                return base.Create(group);
            }
            catch (DataException ex)
            {
                throw new CreateEntityFailedException(ex.Message, ex.InnerException);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> LoadGroupsWithDetailsByAuthorId(long ownerId)
        {
            using (ISession session = GetSession())
            {
                var user = Load<User>(ownerId);
                return
                    session.CreateCriteria<Group>()
                        .Add(Restrictions.Eq("Author", user))
                        .SetFetchMode("Members", FetchMode.Join)
                        .List<Group>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void AssignTaskToGroup(string groupId, string taskId, string userId)
        {
            using (ISession session = GetSession())
            {
                var group = FindById<Group>(long.Parse(groupId));
                var user = Load<User>(long.Parse(userId));
                
                if (!user.Equals(group.Author)) 
                    return;

                var task = FindById<Task>(long.Parse(taskId));
                group.Task = task;
                Update(group);
            }
        }
    }
}