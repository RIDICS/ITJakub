using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class BookPageImageContract
    {
        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public int Position { get; set; }
        
    }
}