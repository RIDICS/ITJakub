using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.Web.DataEntities.Database.Daos;
using ITJakub.Web.DataEntities.Database.Entities;

namespace ITJakub.Web.DataEntities.Database.Repositories
{
    [Transactional]
    public class StaticTextRepository : NHibernateTransactionalDao
    {
        public StaticTextRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }
        
        [Transaction(TransactionMode.Requires)]
        public virtual StaticText GetStaticText(string name)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<StaticText>()
                    .Where(x => x.Name == name)
                    .SingleOrDefault();
            }
        }
    }
}
