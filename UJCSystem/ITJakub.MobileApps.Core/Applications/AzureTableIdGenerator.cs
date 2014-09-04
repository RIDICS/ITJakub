using System;

namespace ITJakub.MobileApps.Core.Applications
{
    public class AzureTableIdGenerator
    {
        public string GetNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}