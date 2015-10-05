using System;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables
{
    public class AzureTableIdGenerator
    {
        public string GetNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}