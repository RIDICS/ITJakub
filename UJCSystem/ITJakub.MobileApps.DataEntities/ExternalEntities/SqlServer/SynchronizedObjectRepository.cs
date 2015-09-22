using System;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.ExternalEntities.SqlServer.Entities;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.SqlServer
{
    [Transactional]
    public class SynchronizedObjectRepository : NHibernateTransactionalDao, ISynchronizedObjectDao
    {
        public SynchronizedObjectRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        public virtual void Save(ISynchronizedObjectEntity entity)
        {
            base.Save(entity);
        }

        public virtual void Delete(string externalObjectId, long groupId)
        {
            using (var session = GetSession())
            {
                var soe = session.Load<SynchronizedObjectData>(Convert.ToInt64(externalObjectId));
                session.Delete(soe);
            }
        }

        public virtual ISynchronizedObjectEntity FindByObjectExternalIdAndGroup(string objectExternalId, long groupId)
        {
            using (var session = GetSession())
            {
                return session.Get<SynchronizedObjectData>(Convert.ToInt64(objectExternalId));
            }
        }

        public virtual ISynchronizedObjectEntity GetNewEntity(long groupId, string data)
        {
            using (var session = GetSession())
            {
                return new SynchronizedObjectData {Group = session.Load<Group>(groupId), Data = data};
            }
        }

        public void DeleteSynchronizedObjects(long groupId, IEnumerable<string> externalIds)
        {
            return;
        }
    }
}