using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables
{
    public class AzureTableSynchronizedObjectDao : AzureTableGenericDao<AzureSynchronizedObjectEntity>, ISynchronizedObjectDao
    {
        private readonly AzureTableIdGenerator m_idGenerator;

        public AzureTableSynchronizedObjectDao(AzureTablesClient azureTablesClient, AzureTableIdGenerator idGenerator)
            : base(azureTablesClient,"SyncObjectTable")
        {
            m_idGenerator = idGenerator;
        }

        public void Save(ISynchronizedObjectEntity entity)
        {
            base.Create((AzureSynchronizedObjectEntity)entity);
        }

        public void Delete(ISynchronizedObjectEntity entity)
        {
            base.Delete((AzureSynchronizedObjectEntity)entity);
        }

        public IEnumerable<ISynchronizedObjectEntity> GetAllGroupId(long groupId)
        {
            return base.GetAllByPartitionKey(Convert.ToString(groupId));
        }

        //public IEnumerable<ISynchronizedObjectEntity> FindAll(IEnumerable<ISynchronizedObjectEntity> tableEntities)
        //{
        //    return base.FindAll(tableEntities);
        //}

        public ISynchronizedObjectEntity FindByObjectExternalIdAndGroup(string externalId, long groupId)
        {

            return FindByRowAndPartitionKey(externalId, Convert.ToString(groupId));
        }        

        public ISynchronizedObjectEntity GetNewEntity(long groupId, string data)
        {
            return new AzureSynchronizedObjectEntity(m_idGenerator.GetNewId(), Convert.ToString(groupId), data);
        }

        public void Delete(string externalObjectId, long groupId)
        {
            Delete(externalObjectId, Convert.ToString(groupId));
        }

    }
}