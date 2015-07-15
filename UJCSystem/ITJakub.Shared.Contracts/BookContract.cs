using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class BookContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public IList<int> CategoryIds { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }

    }
}