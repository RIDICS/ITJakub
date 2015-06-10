using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class PageBookmarkContract
    {
        [DataMember]
        public string Test { get; set; }
    }
}