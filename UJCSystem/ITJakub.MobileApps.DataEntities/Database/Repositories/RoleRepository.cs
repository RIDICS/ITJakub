using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class RoleRepository : NHibernateTransactionalDao<Role>
    {
        public RoleRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }
    }
}