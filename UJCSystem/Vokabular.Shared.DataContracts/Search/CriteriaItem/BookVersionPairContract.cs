using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract]
    public class BookVersionPairContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string VersionId { get; set; }
    }
}