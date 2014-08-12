using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class ApplicationRepository : NHibernateTransactionalDao<Application>
    {
        public ApplicationRepository(ISessionManager sessManager)
            : base(sessManager)
        {
            
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Application> GetAllApplication()
        {
            using (var session = GetSession())
            {
                return session.CreateCriteria<Application>().List<Application>();
            }
        }
    }
}