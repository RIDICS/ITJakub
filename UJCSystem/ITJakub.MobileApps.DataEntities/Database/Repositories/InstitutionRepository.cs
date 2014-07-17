using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class InstitutionRepository : NHibernateTransactionalDao<Institution>
    {
        public InstitutionRepository(ISessionManager sessManager): base(sessManager)
        {
        }
    }
}