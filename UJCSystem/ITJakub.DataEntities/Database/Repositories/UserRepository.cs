using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;

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
            using (ISession session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(user => user.UserName == userName)
                    .SingleOrDefault<User>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User FindById(int userId)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(user => user.Id == userId)
                    .SingleOrDefault<User>();
            }
        }
        
        
        [Transaction(TransactionMode.Requires)]
        public virtual int Create(User user)
        {
            using (ISession session = GetSession())
            {
                return (int) base.Create(user);
            }
        }
    }
}