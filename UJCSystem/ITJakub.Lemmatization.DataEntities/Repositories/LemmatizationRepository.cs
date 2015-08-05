using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;

namespace ITJakub.Lemmatization.DataEntities.Repositories
{
    [Transactional]
    public class LemmatizationRepository : NHibernateTransactionalDao {
        public LemmatizationRepository(ISessionManager sessManager) : base(sessManager)
        {
        }


    }
}