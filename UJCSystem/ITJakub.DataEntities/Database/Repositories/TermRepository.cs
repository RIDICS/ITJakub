using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class TermRepository : NHibernateTransactionalDao
    {
        public TermRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual Term FindByXmlId(string xmlId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Term>()
                    .Where(term => term.XmlId == xmlId)
                    .SingleOrDefault<Term>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual TermCategory GetTermCategoryByName(string termCategory)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<TermCategory>().Where(x => x.Name == termCategory).SingleOrDefault<TermCategory>();                               
            }
        }
    }
}