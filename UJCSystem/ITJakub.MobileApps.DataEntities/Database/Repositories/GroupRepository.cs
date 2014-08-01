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
            catch (DataException ex)
            {
                throw new CreateEntityFailedException(ex.Message, ex.InnerException);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> LoadGroupsWithDetailsByAuthor(User author)
        {
            using (var session = GetSession())
            {
                return
                    session.CreateCriteria<Group>()
                        .Add(Restrictions.Eq("Author", author))
                        .SetFetchMode("Members", FetchMode.Join)
                        .List<Group>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> LoadGroupsWithDetailsByMember(User member)
        {
            using (var session = GetSession())
            {
                //return
                //    session.CreateCriteria<Group>()
                //        .SetFetchMode("Members", FetchMode.Join)
                //        .CreateAlias("Members", "Members")
                //        .Add(Restrictions.In("Members.UserId", member.Id))
                //        .SetFetchMode("Author", FetchMode.Join)
                //        .List<Group>();

                return session.QueryOver<Group>().
                            Right.JoinQueryOver<User>(x => x.Members)
                           .Where(u => u.Id == member.Id)
                           .List();
            }
        }
    }
}