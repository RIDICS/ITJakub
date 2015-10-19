using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class CategoryContentContract
    {
        [DataMember]
        public IList<CategoryContract> Categories { get; set; }

        [DataMember]
        public IList<BookContract> Books { get; set; }

    }
}