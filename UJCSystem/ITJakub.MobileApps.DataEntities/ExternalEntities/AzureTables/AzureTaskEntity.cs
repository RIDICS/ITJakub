using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables
{
    public class AzureTaskEntity : TableEntity, ITaskEntity
    {
        public AzureTaskEntity()
        {
        }

        public AzureTaskEntity(string id, string appId, string data) : this()
        {
            this.PartitionKey = appId;
            this.RowKey = id;
            this.Data = data;
        }

        [IgnoreProperty]
        public string AppId
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        [IgnoreProperty]
        public string Id
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        public string Data { get; set; }
    }
}