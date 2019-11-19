using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.ITJakubService.DataContracts", Name = "HeadwordContract")]
    public class HeadwordContract
    {
        [DataMember]
        public string Headword { get; set; }
        
        [DataMember]
        public IList<HeadwordBookInfoContract> Dictionaries { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.ITJakubService.DataContracts", Name = "HeadwordBookInfoContract")]
    public class HeadwordBookInfoContract
    {
        [DataMember]
        public string EntryXmlId { get; set; }

        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public long? PageId { get; set; }
    }
}