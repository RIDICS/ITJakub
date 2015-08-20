using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace ITJakub.Lemmatization.DataEntities.Repositories
{
    [Transactional]
    public class LemmatizationRepository : NHibernateTransactionalDao {
        public LemmatizationRepository(ISessionManager sessManager) : base(sessManager)
        {
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
        public virtual IList<CanonicalForm> GetTypeaheadCannonicalForm(CanonicalFormType type, string query, int recordCount)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<CanonicalForm>()
                    .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                    .And(x => x.Type == type)
                    .OrderBy(x => x.Text).Asc
                    .Take(recordCount)
                    .List();
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HyperCanonicalForm> GetTypeaheadHyperCannonicalForm(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<HyperCanonicalForm>()
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
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual TokenCharacteristic GetTokenCharacteristicWithCanonicalForms(long tokenCharacteristicId)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<TokenCharacteristic>()
                    .Where(x => x.Id == tokenCharacteristicId)
                    .Fetch(x => x.CanonicalForms).Eager
                    .SingleOrDefault();

                return result;
            }
        }
    }
}