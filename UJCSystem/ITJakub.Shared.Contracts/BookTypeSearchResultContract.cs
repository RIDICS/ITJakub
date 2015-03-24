using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class BookTypeSearchResultContract
    {
        [DataMember]
        public BookTypeEnumContract BookType { get; set; }

        [DataMember]
        public IList<CategoryContract> Categories { get; set; }

        [DataMember]
        public IList<BookContract> Books { get; set; }


    }
}