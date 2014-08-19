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

        public string Data { get; set; }
    }
}