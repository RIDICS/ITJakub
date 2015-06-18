using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.MobileContracts
{
    [DataContract]
    public class BookContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public IList<AuthorContract> Authors { get; set; }

        [DataMember]
        public string PublishDate { get; set; }
    }
}