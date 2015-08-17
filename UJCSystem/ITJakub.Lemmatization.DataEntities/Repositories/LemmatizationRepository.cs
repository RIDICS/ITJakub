using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using NHibernate.Criterion;

namespace ITJakub.Lemmatization.DataEntities.Repositories
{
    [Transactional]
    public class LemmatizationRepository : NHibernateTransactionalDao {
        public LemmatizationRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Token> GetTypeaheadToken(string query, int prefetchRecordCount)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<Token>()
                    .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                    .OrderBy(x => x.Text).Asc
                    .Take(prefetchRecordCount)
                    .List();
                return result;
            }
        }
    }
}