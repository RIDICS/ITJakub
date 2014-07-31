using System;

namespace ITJakub.MobileApps.Client.DataContracts
{
    public class ObjectDetails
    {
        public AuthorInfo Author { get; set; }
        public DateTime CreateTime { get; set; }
        public string Data { get; set; }
        public long Id { get; set; }
    }
}
