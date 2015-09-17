using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

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
        public virtual User FindByUserName(string userName)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(user => user.UserName == userName)
                    .SingleOrDefault<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User FindById(int userId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(user => user.Id == userId)
                    .SingleOrDefault<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int Create(User user)
        {
            using (var session = GetSession())
            {
                return (int) base.Create(user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<User> GetLastUsers(int recordCount)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<User>()
                    .OrderBy(x => x.CreateTime).Desc
                    .Take(recordCount)
                    .List<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<User> GetTypeaheadUsers(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<User>()
                    .WhereRestrictionOn(x => x.UserName).IsInsensitiveLike(query)
                    .Take(recordCount)
                    .List<User>();
            }
        }
    }
}