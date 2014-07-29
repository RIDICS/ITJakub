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
    public class SynchronizedObjectRepository : NHibernateTransactionalDao<SynchronizedObject>
    {
        public SynchronizedObjectRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<SynchronizedObject> LoadSyncObjectsWithDetails(Group group, Application application, string objectType, DateTime since)
        {
            {
                using (var session = GetSession())
                {
                    return
                        session.CreateCriteria<SynchronizedObject>()
                            .Add(Restrictions.Eq("Application", application))
                            .Add(Restrictions.Eq("Group", group))
                            .Add(Restrictions.Eq("ObjectType", objectType))
                            .Add(Restrictions.Gt("CreateTime", since))
                            .SetFetchMode("Author", FetchMode.Join)
                            .List<SynchronizedObject>();
                }
            }
        }
    }
}