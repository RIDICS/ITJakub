using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class HeadwordSearchResultContract
    {
        [DataMember]
        public int HeadwordCount { get; set; }

        [DataMember]
        public int FulltextCount { get; set; }
    }

    [DataContract]
    public class HeadwordContract
    {
        [DataMember]
        public string Headword { get; set; }

        [DataMember]
        public IList<HeadwordBookInfoContract> Dictionaries { get; set; }
    }

    [DataContract]
    public class HeadwordBookInfoContract
    {
        [DataMember]
        public string EntryXmlId { get; set; }

        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public string BookVersionXmlId { get; set; }

        [DataMember]
        public string BookAcronym { get; set; }
    }
}