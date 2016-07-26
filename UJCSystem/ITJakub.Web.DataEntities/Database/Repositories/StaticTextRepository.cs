using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.Web.DataEntities.Database.Daos;
using ITJakub.Web.DataEntities.Database.Entities;

namespace ITJakub.Web.DataEntities.Database.Repositories
{
    public class StaticTextRepository : NHibernateTransactionalDao
    {
        public StaticTextRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }
        
        [Transaction(TransactionScopeOption.Required)]
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
