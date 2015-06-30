using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
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
        public string XmlEntryId { get; set; }

        [DataMember]
        public string BookGuid { get; set; }

        [DataMember]
        public string BookVersionId { get; set; }

        [DataMember]
        public string BookAcronym { get; set; }
    }
}