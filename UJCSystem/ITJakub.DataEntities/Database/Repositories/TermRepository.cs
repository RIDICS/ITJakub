using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class TermRepository : NHibernateTransactionalDao
    {
        public TermRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void SaveOrUpdate(Term term)
        {
            using (var session = GetSession())
            {
                var tmpTerm = FindByXmlId(term.XmlId);
                if (tmpTerm != null)
                {
                    tmpTerm.Position = term.Position;
                    tmpTerm.Text = term.Text;
                    session.SaveOrUpdate(tmpTerm);
                }
                else
                {
                    session.SaveOrUpdate(term);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Term FindByXmlId(string xmlId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Term>()
                    .Where(term => term.XmlId == xmlId)
                    .SingleOrDefault<Term>();
            }
        }
    }
}