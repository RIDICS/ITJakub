using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching", Name = "BookVersionPairContract")]
    public class BookVersionPairContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string VersionId { get; set; }
    }
}