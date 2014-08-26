using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class UsersRepository : NHibernateTransactionalDao
    {
        public UsersRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User LoadUserWithDetails(long id)
        {
            using (ISession session = GetSession())
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

        [Transaction(TransactionMode.Requires)]
        public virtual string GetCommunicationToken(byte authenticationProvider, string authenticationProviderToken)
        {
            using (ISession session = GetSession())
            {
                var user = session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("AuthenticationProvider", authenticationProvider))
                    .Add(Restrictions.Eq("AuthenticationProviderToken", authenticationProviderToken))
                    .UniqueResult<User>();
                return user == null ? null : user.CommunicationToken;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override object Create(object instance)
        {
            try
            {
                return base.Create(instance);
            }
            catch (DataException ex)
            {
                throw new CreateEntityFailedException(ex.Message, ex.InnerException);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User FindByEmailAndProvider(string email, AuthenticationProviders authenticationProvider)
        {
            using (ISession session = GetSession())
            {
                return session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("Email", email))
                    .Add(Restrictions.Eq("AuthenticationProvider", authenticationProvider))
                    .UniqueResult<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User GetUserByCommunicationToken(string communicationToken)
        {
            using (ISession session = GetSession())
            {
                return session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("CommunicationToken", communicationToken))
                    .UniqueResult<User>();
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual User FindByAuthenticationProviderToken(string token)
        {
            using (ISession session = GetSession())
            {
                return session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("AuthenticationProviderToken", token))
                    .UniqueResult<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User LoadGroupsForUser(long userId)
        {
            using (ISession session = GetSession())
            {
                var user = session.CreateCriteria<User>()
                    .Add(Restrictions.Eq(Projections.Id(), userId))
                    .SetFetchMode("MemberOfGroups", FetchMode.Join)
                    .SetFetchMode("MemberOfGroups.Members", FetchMode.Join)
                    .SetFetchMode("CreatedGroups", FetchMode.Join)
                    .SetFetchMode("CreatedGroups.Members", FetchMode.Join)
                    .UniqueResult<User>();
                return user;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User GetUserWithGroups(long userId)
        {
            using (ISession session = GetSession())
            {
                session.QueryOver<Group>()
                    .Where(group => group.Author.Id == userId)
                    .JoinQueryOver<User>(group => group.Members, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();
                
                var u1 = session.QueryOver<User>()
                    .Where(x => x.Id == userId)
                    .Fetch(x => x.CreatedGroups).Eager
                    .JoinQueryOver<Group>(x => x.MemberOfGroups, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                var groupIds = QueryOver.Of<Group>()
                    .Right.JoinQueryOver<User>(x => x.Members)
                    .Where(user => user.Id == userId)
                    .Select(x => x.Id);

                var groupWithMembers = session.QueryOver<Group>()
                    .WithSubquery
                    .WhereProperty(x => x.Id).In(groupIds)
                    .JoinQueryOver<User>(x => x.Members, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                var result = u1.Single();
                result.MemberOfGroups = groupWithMembers.ToList();
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> GetGroupMembers(IEnumerable<long> groupIds)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Group>()
                    .WhereRestrictionOn(group => group.Id).IsIn(groupIds.ToList())
                    .JoinQueryOver<User>(group => group.Members, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();
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
                        .UniqueResult<Group>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group GetGroupDetails(long groupId)
        {
            using (var session = GetSession())
            {
                return session.CreateCriteria<Group>()
                    .Add(Restrictions.Eq(Projections.Id(), groupId))
                    .SetFetchMode("Members", FetchMode.Join)
                    .UniqueResult<Group>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Task> GetTasksByApplication(int applicationId)
        {
            using (var session = GetSession())
            {
                var application = Load<Application>(applicationId);

                return session.CreateCriteria<Task>()
                    .Add(Restrictions.Eq("Application", application))
                    .SetFetchMode("Author", FetchMode.Join)
                    .AddOrder(new Order("CreateTime", false))
                    .List<Task>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Task> GetTasksByAuthor(long authorId)
        {
            using (var session = GetSession())
            {
                var author = Load<User>(authorId);

                return session.CreateCriteria<Task>()
                    .Add(Restrictions.Eq("Author", author))
                    .SetFetchMode("Author", FetchMode.Join)
                    .List<Task>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group GetGroupWithTask(long groupId)
        {
            using (var session = GetSession())
            {
                var group = session.QueryOver<Group>()
                    .Where(x => x.Id == groupId)
                    .Fetch(x => x.Task).Eager
                    .SingleOrDefault();

                return group;
            }
        }
    }
}