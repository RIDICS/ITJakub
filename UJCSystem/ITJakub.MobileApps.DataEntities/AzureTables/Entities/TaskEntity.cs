using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.AzureTables.Entities
{
    public class TaskEntity : TableEntity
    {
        public TaskEntity()
        {
        }

        public TaskEntity(string id, string appId, string data)
        {
            this.PartitionKey = appId;
            this.RowKey = id;
            this.Data = data;
        }

        [IgnoreProperty]
        public string AppId
        {
            get { return this.PartitionKey; }
        }

        [IgnoreProperty]
        public string Id
        {
            get { return this.RowKey; }
        }

        public string Data { get;  set; }
    }
}