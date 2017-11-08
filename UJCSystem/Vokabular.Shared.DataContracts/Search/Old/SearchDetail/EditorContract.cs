using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old.SearchDetail
{
    [DataContract]
    public class EditorContract
    {
        [DataMember]
        public string Text { get; set; }
    }
}