using System;
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
        public UsersRepository(ISessionManager sessManager) : base(sessManager)
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
            catch (DataException)
            {
                throw new CreateEntityFailedException();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User FindByEmailAndProvider(string email, byte authenticationProvider)
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
        public virtual bool IsCommunicationTokenValid(string communicationToken, TimeSpan expirationTime)
        {
            using (var session = GetSession())
            {
                var user= session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("CommunicationToken", communicationToken))
                    .UniqueResult<User>();
                if (user == null) return false;
                return user.CommunicationTokenCreateTime.Add(expirationTime) <= DateTime.UtcNow;
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
    }
}