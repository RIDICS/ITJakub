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

        public T Create<T>(T instance)
        {
            var result = base.Create(instance);
            return (T) result;
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Token> GetTypeaheadToken(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<Token>()
                    .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                    .OrderBy(x => x.Text).Asc
                    .Take(recordCount)
                    .List();
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<CanonicalForm> GetTypeaheadCannonicalForm(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<CanonicalForm>()
                    .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                    .OrderBy(x => x.Text).Asc
                    .Take(recordCount)
                    .List();
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<TokenCharacteristic> GetTokenCharacteristicDetail(long tokenId)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<TokenCharacteristic>()
                    .Where(x => x.Token.Id == tokenId)
                    .Fetch(x => x.CanonicalForms).Eager
                    .List();

                return result;
            }
        }
    }
}