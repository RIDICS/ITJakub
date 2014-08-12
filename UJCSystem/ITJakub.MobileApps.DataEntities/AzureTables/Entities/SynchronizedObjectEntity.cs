using System;
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
            PartitionKey = groupId;
            RowKey = id;
            Data = data;
        }

        [IgnoreProperty]
        public long GroupId
        {
            get { return Convert.ToInt64(PartitionKey); }
        }

        [IgnoreProperty]
        public long Id
        {
            get { return Convert.ToInt64(RowKey); }
        }

        public string Data { get; set; }
    }
}