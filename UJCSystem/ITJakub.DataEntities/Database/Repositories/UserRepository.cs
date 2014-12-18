using System;
using System.Linq;
using System.Security.Cryptography;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using NHibernate;
using NHibernate.Linq;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class UserRepository : NHibernateTransactionalDao
    {
        public UserRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User LoginUserWithLocalAccount(string email, string password)
        {
            using (ISession session = GetSession())
            {
                var userInstance = session.Query<User>().SingleOrDefault(user => user.Email == email && user.AuthenticationProvider == AuthenticationProvider.ItJakub); //TODO use query over
                if (userInstance!=null && userInstance.PasswordHash.Equals(ComputePasswordHash(password, userInstance.Salt)))
                {
                    userInstance.CommunicationToken = GenerateCommunicationToken();
                    userInstance.CommunicationTokenCreateTime = DateTime.UtcNow;
                    Update(userInstance);
                    return userInstance;
                }
                return null;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int RegisterLocalAccount(string email, string password, string firstName, string lastName)
        {
            using (ISession session = GetSession())
            {
                string salt = GenerateSalt();
                DateTime registrationTime = DateTime.UtcNow;
                return (int) Create(new User
                {
                    Email = email,
                    PasswordHash = ComputePasswordHash(password, salt),
                    Salt = salt,
                    FirstName = firstName,
                    LastName = lastName,
                    CreateTime = registrationTime,
                    AuthenticationProvider = AuthenticationProvider.ItJakub,
                    CommunicationToken = GenerateCommunicationToken(),
                    CommunicationTokenCreateTime = registrationTime
                });
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User GetUserByCommunicationToken(string communicationToken)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(user => user.CommunicationToken == communicationToken)
                    .SingleOrDefault<User>();
            }
        }


        private string GenerateCommunicationToken()
        {
            return Guid.NewGuid().ToString();
        }


        private string ComputePasswordHash(string password, string salt)
        {
            string inputString = string.Concat(password, salt); //TODO make better inputString generating
            using (MD5 md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(Convert.FromBase64String(inputString)));
            }
        }

        private string GenerateSalt()
        {
            return Guid.NewGuid().ToString("N"); //TODO seems little bit overpowered
        }
    }
}