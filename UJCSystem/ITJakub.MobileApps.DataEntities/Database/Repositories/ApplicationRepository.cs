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
    public class ApplicationRepository : NHibernateTransactionalDao
    {
        public ApplicationRepository(ISessionManager sessManager)
            : base(sessManager)
        {
            
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Application> GetAllApplication()
        {
            using (var session = GetSession())
            {
                return session.CreateCriteria<Application>().List<Application>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<SynchronizedObject> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            using (var session = GetSession())
            {
                var group = Load<Group>(groupId);
                var application = Load<Application>(applicationId);
                return session.CreateCriteria<SynchronizedObject>()
                        .Add(Restrictions.Eq("Application", application))
                        .Add(Restrictions.Eq("Group", group))
                        .Add(Restrictions.Eq("ObjectType", objectType))
                        .Add(Restrictions.Gt("CreateTime", since))
                        .AddOrder(Order.Asc("CreateTime"))
                        .SetFetchMode("Author", FetchMode.Join)
                        .List<SynchronizedObject>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual SynchronizedObject GetLatestSynchronizedObject(long groupId, int applicationId, string objectType, DateTime since)
        {
            using (var session = GetSession())
            {
                var group = Load<Group>(groupId);
                var application = Load<Application>(applicationId);
                return session.CreateCriteria<SynchronizedObject>()
                    .Add(Restrictions.Eq("Application", application))
                    .Add(Restrictions.Eq("Group", group))
                    .Add(Restrictions.Eq("ObjectType", objectType))
                    .Add(Restrictions.Gt("CreateTime", since))
                    .AddOrder(Order.Desc("CreateTime"))
                    .SetMaxResults(1)
                    .SetFetchMode("Author", FetchMode.Join)
                    .UniqueResult<SynchronizedObject>();
            }
        }
    }
}