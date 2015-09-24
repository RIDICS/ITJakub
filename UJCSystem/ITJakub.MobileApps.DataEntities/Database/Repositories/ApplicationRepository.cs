using System;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;

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
                return session.QueryOver<Application>().List();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<SynchronizedObject> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since, int count)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<SynchronizedObject>()
                    .Where(x => x.Application.Id == applicationId
                        && x.Group.Id == groupId
                        && x.ObjectType == objectType
                        && x.CreateTime > since)
                    .OrderBy(x => x.CreateTime).Asc
                    .Fetch(x => x.Author).Eager
                    .Take(count)
                    .List();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual SingleSynchronizedObject GetLatestSynchronizedObject(long groupId, int applicationId, string objectType, DateTime since)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<SingleSynchronizedObject>()
                    .Where(x => x.Application.Id == applicationId && x.Group.Id == groupId && x.ObjectType == objectType && x.CreateTime > since)
                    .Fetch(x => x.Author).Eager
                    .SingleOrDefault();
            }
        }
    }
}