using ITJakub.MobileApps.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables
{
    public class AzureTablesClient
    {
        private readonly CloudTableClient m_tableClient;

        public AzureTablesClient(IConnectionStringProvider connectionStringProvider)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionStringProvider.GetAzureTableConnectionString());
            m_tableClient = storageAccount.CreateCloudTableClient();
            m_tableClient.DefaultRequestOptions.PayloadFormat = TablePayloadFormat.JsonNoMetadata;
        }

        public CloudTable GetTableReference(string name)
        {
            return m_tableClient.GetTableReference(name);
        }
    }
}