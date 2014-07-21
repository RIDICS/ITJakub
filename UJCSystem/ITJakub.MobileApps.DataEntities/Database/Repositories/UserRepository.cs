using System;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class UserRepository : NHibernateTransactionalDao<User>
    {
        public UserRepository(ISessionManager sessManager): base(sessManager)
        {
        }

        public void CreateUser(User user)
        {
            Create(new User() {FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, CreateTime = DateTime.Now.ToUniversalTime()});    //TODO change after test
        }
    }
}