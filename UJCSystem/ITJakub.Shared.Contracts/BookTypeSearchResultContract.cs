using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

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
        public IList<BookContractWithCategories> Books { get; set; }


    }
}