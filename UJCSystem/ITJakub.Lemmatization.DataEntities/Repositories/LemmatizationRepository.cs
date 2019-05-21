using System.Collections.Generic;
using ITJakub.Lemmatization.DataEntities.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.Lemmatization.DataEntities.Repositories
{
    public class LemmatizationRepository : NHibernateDao
    {
        public LemmatizationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual IList<Token> GetTypeaheadToken(string query, int recordCount)
        {
            var result = GetSession().QueryOver<Token>()
                .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                .OrderBy(x => x.Text).Asc
                .Take(recordCount)
                .List();
            return result;
        }

        public virtual IList<CanonicalForm> GetTypeaheadCannonicalForm(CanonicalFormType type, string query, int recordCount)
        {
            var result = GetSession().QueryOver<CanonicalForm>()
                .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                .And(x => x.Type == type)
                .OrderBy(x => x.Text).Asc
                .Fetch(SelectMode.Fetch, x => x.HyperCanonicalForm)
                .Take(recordCount)
                .List();
            return result;
        }

        public virtual IList<HyperCanonicalForm> GetTypeaheadHyperCannonicalForm(HyperCanonicalFormType type, string query, int recordCount)
        {
            var result = GetSession().QueryOver<HyperCanonicalForm>()
                .WhereRestrictionOn(x => x.Text).IsLike(query, MatchMode.Start)
                .And(x => x.Type == type)
                .OrderBy(x => x.Text).Asc
                .Take(recordCount)
                .List();
            return result;
        }

        public virtual IList<TokenCharacteristic> GetTokenCharacteristicDetail(long tokenId)
        {
            TokenCharacteristic tokenCharacteristicAlias = null;
            CanonicalForm canonicalFormAlias = null;
            HyperCanonicalForm hyperCanonicalFormAlias = null;

            var result = GetSession().QueryOver(() => tokenCharacteristicAlias)
                .JoinQueryOver(x => x.CanonicalForms, () => canonicalFormAlias, JoinType.LeftOuterJoin)
                .JoinQueryOver(x => x.HyperCanonicalForm, () => hyperCanonicalFormAlias, JoinType.LeftOuterJoin)
                .Fetch(SelectMode.Fetch, x => x.CanonicalForms)
                .Where(() => tokenCharacteristicAlias.Token.Id == tokenId)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();

            return result;
        }

        public virtual TokenCharacteristic GetTokenCharacteristicWithCanonicalForms(long tokenCharacteristicId)
        {
            var result = GetSession().QueryOver<TokenCharacteristic>()
                .Where(x => x.Id == tokenCharacteristicId)
                .Fetch(SelectMode.Fetch, x => x.CanonicalForms)
                .SingleOrDefault();

            return result;
        }

        public virtual int GetTokenCount()
        {
            var result = GetSession().QueryOver<Token>()
                .RowCount();

            return result;
        }

        public virtual IList<Token> GetTokenList(int start, int count)
        {
            var result = GetSession().QueryOver<Token>()
                .OrderBy(x => x.Text).Asc
                .Take(count)
                .Skip(start)
                .List();

            return result;
        }

        public virtual IList<long> GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            var result = GetSession().QueryOver<CanonicalForm>()
                .Where(x => x.HyperCanonicalForm.Id == hyperCanonicalFormId)
                .Select(x => x.Id)
                .OrderBy(x => x.Text).Asc
                .List<long>();

            return result;
        }

        public virtual CanonicalForm GetCanonicalFormDetail(long canonicalFormId)
        {
            Token tokenAlias = null;
            TokenCharacteristic tokenCharacteristicAlias = null;

            var result = GetSession().QueryOver<CanonicalForm>()
                .Where(x => x.Id == canonicalFormId)
                .Fetch(SelectMode.Fetch, x => x.CanonicalFormFor)
                .JoinAlias(x => x.CanonicalFormFor, () => tokenCharacteristicAlias, JoinType.LeftOuterJoin)
                .JoinAlias(x => tokenCharacteristicAlias.Token, () => tokenAlias, JoinType.LeftOuterJoin)
                .SingleOrDefault();

            return result;
        }
    }
}