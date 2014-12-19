using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class EditorContract
    {
        [DataMember]
        public string Text { get; set; }
    }
}