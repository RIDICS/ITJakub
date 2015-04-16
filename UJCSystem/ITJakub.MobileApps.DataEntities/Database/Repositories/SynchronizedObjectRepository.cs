using System;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class SynchronizedObjectRepository : NHibernateTransactionalDao<SynchronizedObjectBase>
    {
        public SynchronizedObjectRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<SynchronizedObjectBase> LoadSyncObjectsWithDetails(Group group, Application application, string objectType, DateTime since)
        {
            using (var session = GetSession())
            {
                return
                    session.CreateCriteria<SynchronizedObjectBase>()
                        .Add(Restrictions.Eq("Application", application))
                        .Add(Restrictions.Eq("Group", group))
                        .Add(Restrictions.Eq("ObjectType", objectType))
                        .Add(Restrictions.Gt("CreateTime", since))
                        .SetFetchMode("Author", FetchMode.Join)
                        .List<SynchronizedObjectBase>();
            }
        }
    }
}