using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
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
        public virtual IList<HyperCanonicalForm> GetTypeaheadHyperCannonicalForm(HyperCanonicalFormType type, string query, int recordCount)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<HyperCanonicalForm>()
                    .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                    .And(x => x.Type == type)
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
                TokenCharacteristic tokenCharacteristicAlias = null;
                CanonicalForm canonicalFormAlias = null;
                HyperCanonicalForm hyperCanonicalFormAlias = null;

                var result = session.QueryOver(() => tokenCharacteristicAlias)
                    .JoinQueryOver(x => x.CanonicalForms, () => canonicalFormAlias, JoinType.LeftOuterJoin)
                    .JoinQueryOver(x => x.HyperCanonicalForm, () => hyperCanonicalFormAlias, JoinType.LeftOuterJoin)
                    .Fetch(x => x.CanonicalForms).Eager
                    .Where(() => tokenCharacteristicAlias.Token.Id == tokenId)
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

        [Transaction(TransactionMode.Requires)]
        public virtual int GetTokenCount()
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<Token>()
                    .RowCount();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Token> GetTokenList(int start, int count)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<Token>()
                    .OrderBy(x => x.Text).Asc
                    .Take(count)
                    .Skip(start)
                    .List();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<long> GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<CanonicalForm>()
                    .Where(x => x.HyperCanonicalForm.Id == hyperCanonicalFormId)
                    .Select(x => x.Id)
                    .OrderBy(x => x.Text).Asc
                    .List<long>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual CanonicalForm GetCanonicalFormDetail(long canonicalFormId)
        {
            using (var session = GetSession())
            {
                Token tokenAlias = null;
                TokenCharacteristic tokenCharacteristicAlias = null;

                var result = session.QueryOver<CanonicalForm>()
                    .Where(x => x.Id == canonicalFormId)
                    .Fetch(x => x.CanonicalFormFor).Eager
                    .JoinAlias(x => x.CanonicalFormFor, () => tokenCharacteristicAlias, JoinType.LeftOuterJoin)
                    .JoinAlias(x => tokenCharacteristicAlias.Token, () => tokenAlias, JoinType.LeftOuterJoin)
                    .SingleOrDefault();

                return result;
            }
        }
    }
}