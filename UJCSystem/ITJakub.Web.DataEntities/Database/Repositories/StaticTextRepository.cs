using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.Web.DataEntities.Database.Daos;
using ITJakub.Web.DataEntities.Database.Entities;
using NHibernate;

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
        public virtual IList<StaticText> GetAllTexts()
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<StaticText>().List();
            }
        }
    }
}
