using System;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables
{
    public class AzureTableTaskDao : AzureTableGenericDao<AzureTaskEntity>, ITaskDao
    {
        public AzureTableTaskDao(AzureTablesClient azureTablesClient) : base(azureTablesClient, "TaskTable")
        {
        }

        public ITaskEntity GetNewEntity(long taskId, int appId, string data)
        {
            return new AzureTaskEntity(Convert.ToString(taskId), Convert.ToString(appId), data);
        }

        public void Save(ITaskEntity taskEntity)
        {
            Create((AzureTaskEntity) taskEntity);
        }

        public ITaskEntity FindByIdAndAppId(long taskId, int appId)
        {
            return FindByRowAndPartitionKey(Convert.ToString(taskId), Convert.ToString(appId));
        }
    }
    
}