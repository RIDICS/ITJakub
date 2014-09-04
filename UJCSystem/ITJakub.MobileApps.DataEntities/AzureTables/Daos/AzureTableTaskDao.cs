using ITJakub.MobileApps.DataEntities.AzureTables.Entities;

namespace ITJakub.MobileApps.DataEntities.AzureTables.Daos
{
    public class AzureTableTaskDao : AzureTableGenericDao<TaskEntity>
    {
        public AzureTableTaskDao(AzureTablesClient azureTablesClient) : base(azureTablesClient,"TaskTable")
        {
        }
    }
}