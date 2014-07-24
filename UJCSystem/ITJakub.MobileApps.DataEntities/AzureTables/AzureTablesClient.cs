using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.AzureTables
{
    public class AzureTablesClient
    {
        private readonly CloudTableClient m_tableClient;

        public AzureTablesClient(string azureTableConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(azureTableConnectionString);
            m_tableClient = storageAccount.CreateCloudTableClient();
            m_tableClient.DefaultRequestOptions.PayloadFormat = TablePayloadFormat.JsonNoMetadata;
        }

        public CloudTable GetTableReference(string name)
        {
            return m_tableClient.GetTableReference(name);
        }
    }
}