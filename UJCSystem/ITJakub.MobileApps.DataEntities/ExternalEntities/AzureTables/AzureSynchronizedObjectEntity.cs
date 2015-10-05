using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables
{
    public class AzureSynchronizedObjectEntity : TableEntity, ISynchronizedObjectEntity
    {
        public AzureSynchronizedObjectEntity(string id, string groupId, string data):this()
        {
            PartitionKey = groupId;
            RowKey = id;
            Data = data;
        }

        public AzureSynchronizedObjectEntity()
        {            
        }

        [IgnoreProperty]
        public string ExternalId
        {
            get { return RowKey; }
        }
     

        [IgnoreProperty]
        public long GroupId
        {
            get { return Convert.ToInt64(PartitionKey); }
        }

        public string Data { get; set; }
    }
}