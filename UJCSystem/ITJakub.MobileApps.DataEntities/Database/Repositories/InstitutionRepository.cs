using System;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class InstitutionRepository : NHibernateTransactionalDao<Institution>
    {
        public InstitutionRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Institution LoadInstitutionWithDetails(long id)
        {
            using (ISession session = GetSession())
            {
                return session.CreateCriteria<Institution>().Add(Restrictions.Eq(Projections.Id(), id)).SetFetchMode("Members", FetchMode.Join).UniqueResult<Institution>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Institution FindByEnterCode(string enterCode)
        {
            using (ISession session = GetSession())
            {
                return session.CreateCriteria<Institution>().Add(Restrictions.Eq("EnterCode", enterCode)).SetFetchMode("Members", FetchMode.Join).UniqueResult<Institution>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override object Create(Institution instance)
        {
            try
            {
                return base.Create(instance);
            }
            catch (DataException)
            {
                throw new CreateEntityFailedException();
            }
        }
    }
}