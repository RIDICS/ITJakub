using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(BookContractWithCategories))]
    public class BookContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }
    }

    [DataContract]
    public class BookContractWithCategories : BookContract
    {
        [DataMember]
        public IList<int> CategoryIds { get; set; }
    }

    [DataContract]
    public class BookIdContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Guid { get; set; }
    }
}