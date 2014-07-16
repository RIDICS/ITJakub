using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class TestRepository : NHibernateTransactionalDao
    {
        public TestRepository(ISessionManager sessManager) : base(sessManager)
        {
        }


        [Transaction(TransactionMode.Requires)]
        public virtual void TestMethod()
        {

        }
    }
}
