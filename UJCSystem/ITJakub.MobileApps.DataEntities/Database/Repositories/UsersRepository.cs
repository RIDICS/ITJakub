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
                IEnumerable<User> u1 = session.CreateCriteria<User>()
                    .Add(Restrictions.Eq(Projections.Id(), userId))
                    .SetFetchMode("CreatedGroups", FetchMode.Eager)
                    .Future<User>();

                var group = session.CreateCriteria<Group>()
                    .Add(Restrictions.Eq("Author.Id", userId))
                    .SetFetchMode("Members", FetchMode.Join)
                    .Future<Group>();

                session.CreateCriteria<User>()
                    .Add(Restrictions.Eq(Projections.Id(), userId))
                    .SetFetchMode("MemberOfGroups", FetchMode.Eager)
                    .Future<User>();

                var memberOfGroup = session.CreateCriteria<Group>()
                    .CreateAlias("Members", "m", JoinType.LeftOuterJoin)
                    .Add(Restrictions.Eq("m.Id", userId))
                    .SetFetchMode("Members", FetchMode.Join)
                    .Future<Group>();


                return u1.ToList().FirstOrDefault();
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
    }
}