using ITJakub.MobileApps.DataEntities.AzureTables.Entities;

namespace ITJakub.MobileApps.DataEntities.AzureTables.Daos
{
    public class AzureTableSynchronizedObjectDao : AzureTableGenericDao<SynchronizedObjectEntity>
    {
        public AzureTableSynchronizedObjectDao(AzureTablesClient azureTablesClient)
            : base(azureTablesClient,"SyncObjectTable")
        {
        }
    }
}