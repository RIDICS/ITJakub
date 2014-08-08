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
    public class UsersRepository : NHibernateTransactionalDao<User>
    {
        public UsersRepository(ISessionManager sessManager)
            : base(sessManager)
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

        [Transaction(TransactionMode.Requires)]
        public virtual string GetCommunicationToken(byte authenticationProvider, string authenticationProviderToken)
        {
            using (var session = GetSession())
            {
                var user = session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("AuthenticationProvider", authenticationProvider))
                    .Add(Restrictions.Eq("AuthenticationProviderToken", authenticationProviderToken))
                    .UniqueResult<User>();
                return user == null ? null : user.CommunicationToken;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override object Create(User instance)
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
            using (var session = GetSession())
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
            using (var session = GetSession())
            {
                return session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("CommunicationToken", communicationToken))
                    .UniqueResult<User>();
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual User FindByAuthenticationProviderToken(string token)
        {
            using (var session = GetSession())
            {
                return session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("AuthenticationProviderToken", token))
                    .UniqueResult<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User LoadGroupsForUser(long userId)
        {
            using (var session = GetSession())
            {
                User user = session.CreateCriteria<User>()
                    .Add(Restrictions.Eq(Projections.Id(), userId))
                    .SetFetchMode("MemberOfGroups", FetchMode.Join)
                    .SetFetchMode("MemberOfGroups.Members", FetchMode.Join)
                    .SetFetchMode("CreatedGroups", FetchMode.Join)
                    .SetFetchMode("CreatedGroups.Members", FetchMode.Join)
                    .UniqueResult<User>();
                return user;
            }
        }
    }
}