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
                Group createdGroupAlias = null;
                User createdGroupMemberAlias = null;
                
                var u1 = session.QueryOver<User>()
                     .JoinAlias(x => x.CreatedGroups, () => createdGroupAlias, JoinType.LeftOuterJoin)
                    .JoinAlias(() => createdGroupAlias.Members, () => createdGroupMemberAlias, JoinType.LeftOuterJoin)
                    .Where(Restrictions.Eq(Projections.Property<User>(u => u.Id), userId))
                    .Fetch(user => user.CreatedGroups).Eager.TransformUsing(Transformers.DistinctRootEntity).Future<User>();

                //var group =
                //    session.QueryOver<Group>()
                //        .Where(Restrictions.Eq(Projections.Property<Group>(g => g.Author.Id), userId))
                //        .Fetch(x => x.Members).Eager.TransformUsing(Transformers.DistinctRootEntity).Future();

                
                var u2 = session.QueryOver<User>()
                    .Where(x => x.Id == userId)
                    .JoinQueryOver<Group>(x => x.MemberOfGroups).TransformUsing(Transformers.DistinctRootEntity).Future();

                var groupIds =
                    QueryOver.Of<Group>()
                        .Right.JoinQueryOver<User>(x => x.Members).Where(user => user.Id == userId).Select(x => x.Id);


                var groupWithMembers = session.QueryOver<Group>()
                    .WithSubquery
                    .WhereProperty(x => x.Id).In(groupIds)
                    .JoinQueryOver<User>(x => x.Members, JoinType.LeftOuterJoin).TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                var result = u2.Single();
                result.MemberOfGroups = groupWithMembers.ToList();
                return result;
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
    }
}