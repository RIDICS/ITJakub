using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.AzureTables.Entities
{
    public class SynchronizedObjectEntity : TableEntity
    {
        public SynchronizedObjectEntity()
        {
        }

        public SynchronizedObjectEntity(string id, string groupId, string data)
        {
            this.PartitionKey = groupId;
            this.RowKey = id;
            this.Data = data;
        }

        [IgnoreProperty]
        public string GroupId
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